using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;

namespace Ignition.ViewModels
{
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Ignition.Api;
    using Ignition.Models;
    using Ignition.Views;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive;
    using System.Runtime.InteropServices;

    public class LandingWindowViewModel : BaseViewModel
    {
        private PrimaryWindowViewModel primaryWindowViewModel;

        public ReactiveCommand<Unit, Unit> Hangar { get; }
        public ReactiveCommand<Unit, Unit> Achievements { get; }
        public ReactiveCommand<Unit, Unit> Announcements { get; }
        public ReactiveCommand<Unit, Task> Launch { get; }
        public ReactiveCommand<Unit, Unit> SettingsPanel { get; }

        public ReactiveCommand<string, Unit> ModNewsItem { get; }
        public ReactiveCommand<string, Unit> SiriusNewsItem { get; }

        [Reactive]
        public List<NewsItem> SiriusNews { get; set; }

        [Reactive]
        public List<NewsItem> ModNews { get; set; }

        [Reactive]
        public User User { get; set; }

        [Reactive]
        public string GameInstalled { get; set; }

        public LandingWindowViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Hangar = ReactiveCommand.Create(HangarButtonClick);
            Achievements = ReactiveCommand.Create(AchievementsButtonClick);
            Announcements = ReactiveCommand.Create(AnnouncementsButtonClick);
            SettingsPanel = ReactiveCommand.Create(SettingsPanelClick);
            Launch = ReactiveCommand.Create(LaunchButtonClick);
            ModNewsItem = ReactiveCommand.Create<string>(ModNewsClick);
            SiriusNewsItem = ReactiveCommand.Create<string>(SiriusNewsClick);

            SiriusNews = primaryWindowViewModel.SiriusNews;

            ModNews = primaryWindowViewModel.ModNews;

            User = primaryWindowViewModel.LoggedUser;

            GameInstalled = this.primaryWindowViewModel.GameInstalled ? "Launch" : "Download";
        }

        public static uint StringToFnvHash(string str)
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

        private Dictionary<uint, KeyValuePair<string, byte[]>> ComputedHashes { get; set; }

        private async Task PrepareGameUpdates(bool initialDownload)
        {
            // TODO: Show splash screen upon preperation. Remove when done.
            string fileDir = Settings.Instance.LauncherData.AftermathInstall;

            ComputedHashes = new Dictionary<uint, KeyValuePair<string, byte[]>>();
            string[] ignoredDirs = new[] { "SAVES", "SCREENSHOTS", "TOOLS", ".GIT", ".GITLAB" };

            foreach (string dir in Directory.EnumerateDirectories(fileDir, "*", SearchOption.AllDirectories).Where(x => ignoredDirs.All(dir => !x.ToUpper().Contains(dir))))
            {
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
                        ComputedHashes.Add(nameHash, new KeyValuePair<string, byte[]>(relativeDir, hash));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to add hash to list. Likely collision.\nHash: {nameHash}\nPath: {relativeDir}\nEx: {ex.Message}");
                    }
                }
            }

            // TODO: Delete all files that **DO NOT** match the checksum.
            // This way all files will always match the remote client.
            WebClient client = new WebClient();
            var checksums = (await Utils.Utils.GetRequest("/api/game/integrity")).Value.ToObject<Dictionary<uint, KeyValuePair<string, byte[]>>>();
            foreach (var checksum in checksums)
            {
                Directory.CreateDirectory(fileDir + "/" + Path.GetDirectoryName(checksum.Value.Key));
                if (!File.Exists(fileDir + "/" + checksum.Value.Key))
                {
                    // ReSharper disable once MethodHasAsyncOverload
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
                    if (ComputedHashes.Keys.Contains(Convert.ToUInt32(checksum.Key)))
                    {
                        continue;
                    }

                    try
                    {
                        // ReSharper disable once MethodHasAsyncOverload
                        client.DownloadFile(
                            new Uri(Settings.Instance.LauncherData.PatchServer + "/" + checksum.Value.Key),
                            fileDir + "/" + checksum.Value.Key);
                    }
                    catch
                    {
                        // TODO: Handle
                    }
                }
            }

            // TODO: If initial download show message box upon completion.
        }

        private async Task LaunchButtonClick()
        {
            if (!this.primaryWindowViewModel.GameInstalled)
            {
                await PrepareGameUpdates(true);
                return;
            }

            await PrepareGameUpdates(false);

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
#if _WINDOWS
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
                int w = int.Parse(match.Groups[1].Value);
                int h = int.Parse(match.Groups[2].Value);
#else
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
                    int w = int.Parse(match.Groups[1].Value);
                    int h = int.Parse(match.Groups[2].Value);
                }
#endif
                Settings.Instance.SetResolution(w, h);
                // Debug.WriteLine($"{devMode.dmPelsWidth} x {devMode.dmPelsHeight}");
            }
            else
            {
                // Debug.WriteLine($"{Settings.Instance.LauncherData.WidthResolution} x {Settings.Instance.LauncherData.HeightResolution}");
            }

            // TODO: Make sure player id, player sig, and player code are passed into the game correctly.
            // -PlayerId=
            // -AccCode=
            // -AccSig=
            Process[] pname = Process.GetProcessesByName("freelancer");
            if (pname.Length == 0)
            {
                // TODO: Hide splash screen, and unhardcode path. Should be relative to the 
                Process.Start(@"D:\Aftermath\EXE\Freelancer.exe", string.Join(" ", exeArguments.ToArray()));
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
                    ContentTitle = "Freelancer Already Running?",
                    Icon = Icon.Warning,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
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
                    // TODO: Start Freelancer.
                }
                else if (result == terminate)
                {
                    // TODO: Termiante Freelancer process. Cancel if lacking permissions to do so.
                }
                else
                {
                    return;
                }
            }

            //TODO: Make sure that it pull the ID successfully or not launch
        }

        private void StartFreelancer()
        {

        }

        private void SettingsPanelClick()
        {
            SettingsWindow settingsWindow = new SettingsWindow();

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                settingsWindow.ShowDialog(desktopLifetime.MainWindow);
            }
            else if (Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                settingsWindow.ShowDialog((Avalonia.Controls.Window)singleView.MainView);
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

        [StructLayout(LayoutKind.Sequential)]
        struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }
}