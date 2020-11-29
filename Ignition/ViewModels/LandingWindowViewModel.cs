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
    using System.Net;
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

        private static uint StringToFnvHash(string str)
        {
            const uint fnvPrime32 = 16777619;
            const uint fnvOffset32 = 2166136261;

            IEnumerable<byte> bytesToHash = str.ToCharArray().Select(Convert.ToByte);
            uint hash = fnvOffset32;

            foreach (var chunk in bytesToHash)
            {
                hash ^= chunk;
                hash *= fnvPrime32;
            }

            return hash;
        }

        private async Task PrepareGameUpdates(bool initialDownload, CancellationToken cancellationToken)
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

            computedHashes = new Dictionary<uint, KeyValuePair<string, byte[]>>();

            WebClient client = new WebClient
            {
                Proxy = null
            };

            string fileDir = Settings.Instance.LauncherData.AftermathInstall;

            string[] ignoredDirs = new[] { "SAVES", "SCREENSHOTS", "TOOLS", ".GIT", ".GITLAB" };

            var checksums = (await Utils.Utils.GetRequest("/api/game/integrity")).Value.ToObject<Dictionary<uint, KeyValuePair<string, byte[]>>>();

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }


            CurrentAction = "Checking file integrity...";
            this.RaisePropertyChanged(nameof(CurrentAction));

            double fileIndex;

            foreach (string dir in Directory.EnumerateDirectories(fileDir, "*", SearchOption.AllDirectories).Where(x => ignoredDirs.All(dir => !x.ToUpper().Contains(dir))))
            {
                fileIndex = 1;

                foreach (string file in Directory.GetFiles(dir))
                {
                    await using FileStream fs = new FileStream(file, FileMode.Open);
                    await using BufferedStream bs = new BufferedStream(fs, 10 * 1024);
                    using SHA1Managed sha1 = new SHA1Managed();

                    byte[] hash = sha1.ComputeHash(bs);
                    string relativeDir = Path.GetRelativePath(fileDir, file);
                    uint nameHash = StringToFnvHash(relativeDir);
                    try
                    {
                        computedHashes.Add(nameHash, new KeyValuePair<string, byte[]>(relativeDir, hash));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to add hash to list. Likely collision.\nHash: {nameHash}\nPath: {relativeDir}\nEx: {ex.Message}");
                    }

                    Progress = fileIndex / Directory.GetFiles(dir).Length * 100;
                    this.RaisePropertyChanged(nameof(Progress));

                    ProgressString = $"{fileIndex}/{Directory.GetFiles(dir).Length}";
                    this.RaisePropertyChanged(nameof(ProgressString));

                    CurrentFile = relativeDir;
                    this.RaisePropertyChanged(nameof(CurrentFile));

                    fileIndex++;
                }
            }

            CurrentAction = "Downloading files...";
            this.RaisePropertyChanged(nameof(CurrentAction));

            fileIndex = 1;

            foreach (var checksum in checksums)
            {
                Directory.CreateDirectory(fileDir + "/" + Path.GetDirectoryName(checksum.Value.Key));

                if (!File.Exists(fileDir + "/" + checksum.Value.Key))
                {
                    try
                    {
                        client.DownloadFile(
                            new Uri(Settings.Instance.LauncherData.PatchServer + "/" + checksum.Value.Key),
                            fileDir + "/" + checksum.Value.Key);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    if (computedHashes.Keys.Contains(checksum.Key))
                    {
                        Debug.WriteLine("Test");
                        continue;
                    }

                    try
                    {
                        client.DownloadFile(
                            new Uri(Settings.Instance.LauncherData.PatchServer + "/" + checksum.Value.Key),
                            fileDir + "/" + checksum.Value.Key);
                    }
                    catch
                    {
                        // TODO: Handle
                    }
                }

                Progress = fileIndex / checksums.Count * 100;
                this.RaisePropertyChanged(nameof(Progress));

                ProgressString = $"{fileIndex}/{checksums.Count}";
                this.RaisePropertyChanged(nameof(ProgressString));

                CurrentFile = checksum.Value.Key;
                this.RaisePropertyChanged(nameof(CurrentFile));

                fileIndex++;
            }

            foreach (var computedHash in computedHashes)
            {
                if (!checksums.Values.Contains(computedHash.Value))
                {
                    File.Delete($"{fileDir}/{computedHash.Value.Key}");
                }
            }

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
                Task.Run(new Action(async () => await PrepareGameUpdates(true, cancellationToken.Token))).Wait();
                return;
            }

            await Task.Factory.StartNew(() => PrepareGameUpdates(false, cancellationToken.Token).Wait());

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
            else
            {
                progressWindow.Close();
            }
        }
    }
}