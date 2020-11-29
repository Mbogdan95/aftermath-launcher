namespace Ignition.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Ignition.Api;
    using Ignition.Models;
    using Ignition.Views;
    using MessageBox.Avalonia;
    using MessageBox.Avalonia.DTO;
    using MessageBox.Avalonia.Enums;
    using MessageBox.Avalonia.Models;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    public class LandingWindowViewModel : BaseViewModel
    {
        private PrimaryWindowViewModel primaryWindowViewModel;
        private Dictionary<uint, KeyValuePair<string, byte[]>> computedHashes { get; set; }
        private ProgressWindow progressWindow;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public ReactiveCommand<Unit, Unit> Hangar { get; }
        public ReactiveCommand<Unit, Unit> Achievements { get; }
        public ReactiveCommand<Unit, Unit> Announcements { get; }
        public ReactiveCommand<Unit, Task> Launch { get; }
        public ReactiveCommand<string, Unit> ModNewsItem { get; }
        public ReactiveCommand<string, Unit> SiriusNewsItem { get; }
        public ReactiveCommand<Unit, Unit> ProgressWindowButton { get; }

        [Reactive]
        public List<NewsItem> SiriusNews { get; set; }

        [Reactive]
        public List<NewsItem> ModNews { get; set; }

        [Reactive]
        public User User { get; set; }

        [Reactive]
        public string GameInstalled { get; set; }

        public string ProgressType { get; set; }

        public string CurrentAction { get; set; }

        public string CurrentFile { get; set; }

        public string ProgressString { get; set; }

        public string ProgressWindowButtonName { get; set; } = "Cancel";

        public double Progress { get; set; }

        public LandingWindowViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Hangar = ReactiveCommand.Create(HangarButtonClick);
            Achievements = ReactiveCommand.Create(AchievementsButtonClick);
            Announcements = ReactiveCommand.Create(AnnouncementsButtonClick);
            Launch = ReactiveCommand.Create(LaunchButtonClick);
            ModNewsItem = ReactiveCommand.Create<string>(ModNewsClick);
            SiriusNewsItem = ReactiveCommand.Create<string>(SiriusNewsClick);
            ProgressWindowButton = ReactiveCommand.Create(ProgressWindowButtonClick);

            SiriusNews = primaryWindowViewModel.SiriusNews;

            ModNews = primaryWindowViewModel.ModNews;

            User = primaryWindowViewModel.LoggedUser;

            GameInstalled = this.primaryWindowViewModel.GameInstalled ? "Launch" : "Download";
        }

        private async Task PrepareGameUpdates(bool initialDownload)
        {
            if (initialDownload)
            {
                ProgressType = "DOWNLOADING GAME...";
                this.RaisePropertyChanged(nameof(ProgressType));
            }
            else
            {
                ProgressType = "PATCHING GAME...";
                this.RaisePropertyChanged(nameof(ProgressType));
            }

            void CurrentActionChanged(string str)
            {
                CurrentAction = str;
                this.RaisePropertyChanged(nameof(CurrentAction));
            }

            void CurrentProgressChanged(double val, string progressString, string currentFile)
            {
                Progress = val;
                this.RaisePropertyChanged(nameof(Progress));

                ProgressString = progressString;
                this.RaisePropertyChanged(nameof(ProgressString));

                CurrentFile = currentFile;
                this.RaisePropertyChanged(nameof(CurrentFile));
            }

            await Downloader.IntegrityCheck(CurrentActionChanged, CurrentProgressChanged, this.cancellationToken);

            if (initialDownload)
            {
                ProgressWindowButtonName = "OK";

                CurrentAction = "Download complete";
                this.RaisePropertyChanged(nameof(CurrentAction));

                ProgressString = string.Empty;
                this.RaisePropertyChanged(nameof(ProgressString));

                CurrentFile = string.Empty;
                this.RaisePropertyChanged(nameof(CurrentFile));
            }
        }

        private async Task LaunchButtonClick()
        {
            progressWindow = new ProgressWindow
            {
                DataContext = this
            };

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                progressWindow.ShowDialog(desktopLifetime.MainWindow);
            }
            else if (Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                progressWindow.ShowDialog((Window)singleView.MainView);
            }

            if (!primaryWindowViewModel.GameInstalled)
            {
                Task.Run(new Action(async () => await PrepareGameUpdates(true))).Wait();
                return;
            }

            await Task.Factory.StartNew(() => PrepareGameUpdates(false).Wait());

            progressWindow.Close();
            Debug.WriteLine("Test");

            List<string> exeArguments = new List<string>()
            {
                "-server.zoneruniverse.com:2307"
            };

            if (Settings.Instance.LauncherData.WindowedMode)
            {
                exeArguments.Add("-w");
            }

            if (Settings.Instance.LauncherData.DefaultDesktopResolution)
            {
                int w = 0;
                int h = 0;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = "wmic";
                    p.StartInfo.Arguments = "desktopmonitor get screenheight, screenwidth";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    var match = System.Text.RegularExpressions.Regex.Match(output, @".*?\n(\d+).*?(\d+)");
                    w = int.Parse(match.Groups[1].Value);
                    h = int.Parse(match.Groups[2].Value);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Use xrandr to get size of screen located at offset (0,0).
                    Process p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = "xrandr";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    if (output.Contains("'xrandr' not found"))
                    {
                        // Handle
                    }
                    else
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(output, @"(\d+)x(\d+)\+0\+0");
                        w = int.Parse(match.Groups[1].Value);
                        h = int.Parse(match.Groups[2].Value);
                    }
                }

                Settings.Instance.SetResolution(w, h);
            }
            else
            {
            }

            if (!string.IsNullOrEmpty(primaryWindowViewModel.LoggedUser.PlayerID))
            {
                exeArguments.Add($"-PlayerId-{primaryWindowViewModel.LoggedUser.PlayerID}");
            }
            else
            {
                return;
            }

            if (primaryWindowViewModel.LoggedUser.AccCode != 0)
            {
                exeArguments.Add($"-AccCode-{primaryWindowViewModel.LoggedUser.AccCode}");
            }
            else
            {
                return;
            }

            if (primaryWindowViewModel.LoggedUser.AccSig != 0)
            {
                exeArguments.Add($"-AccSig-{primaryWindowViewModel.LoggedUser.AccSig}");
            }
            else
            {
                return;
            }

            Process[] processesNames = Process.GetProcessesByName("freelancer");
            if (processesNames.Length == 0)
            {
                Process.Start($@"{Settings.Instance.LauncherData.AftermathInstall}\EXE\Freelancer.exe", string.Join(" ", exeArguments.ToArray()));
            }
            else
            {
                string stop = "Stop Launching Freelancer";
                string cont = "Start Anyway";
                string terminate = "Terminate & Start New";

                var result = await MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
                {
                    Style = Style.UbuntuLinux,
                    ContentMessage = "A copy of Freelancer appears to already be running. Would you like to terminate this running process?\n",
                    ContentHeader = "Freelancer Already Running?",
                    Icon = Icon.Warning,
                    ShowInCenter = true,
                    ButtonDefinitions = new[]
                    {
                        new ButtonDefinition { Name = stop },
                        new ButtonDefinition { Name = cont },
                        new ButtonDefinition { Name = terminate, Type = ButtonType.Colored }
                    }
                }).Show();

                if (result == cont)
                {
                    Process.Start($@"{Settings.Instance.LauncherData.AftermathInstall}\EXE\Freelancer.exe", string.Join(" ", exeArguments.ToArray()));
                }
                else if (result == terminate)
                {
                    processesNames[0].Kill();

                    Process.Start($@"{Settings.Instance.LauncherData.AftermathInstall}\EXE\Freelancer.exe", string.Join(" ", exeArguments.ToArray()));
                }
                else
                {
                    return;
                }
            }
        }

        private void AnnouncementsButtonClick()
        {
            Debug.WriteLine("Announcements Button Click");
        }

        private void AchievementsButtonClick()
        {
            primaryWindowViewModel.OnUpdateView("Achievements");
        }

        private void HangarButtonClick()
        {
            primaryWindowViewModel.OnUpdateView("Hangar");
        }

        private void ModNewsClick(string newsTitle)
        {
            NewsItem newsItemClicked = ModNews.Find(x => x.Title == newsTitle);

            try
            {
                Process.Start(newsItemClicked.NewsUrl);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    newsItemClicked.NewsUrl = newsItemClicked.NewsUrl.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {newsItemClicked.NewsUrl}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", newsItemClicked.NewsUrl);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", newsItemClicked.NewsUrl);
                }
                else
                {
                    throw;
                }
            }
        }

        private void SiriusNewsClick(string newsTitle)
        {
            NewsItem newsItemClicked = SiriusNews.Find(x => x.Title == newsTitle);

            try
            {
                Process.Start(newsItemClicked.NewsUrl);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    newsItemClicked.NewsUrl = newsItemClicked.NewsUrl.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {newsItemClicked.NewsUrl}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", newsItemClicked.NewsUrl);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", newsItemClicked.NewsUrl);
                }
                else
                {
                    throw;
                }
            }
        }

        private void ProgressWindowButtonClick()
        {
            if (ProgressWindowButtonName == "Cancel")
            {
                cancellationToken.Cancel();
            }
            progressWindow.Close();
        }
    }
}