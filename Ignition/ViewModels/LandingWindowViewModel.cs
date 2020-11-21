namespace Ignition.ViewModels
{
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Ignition.Api;
    using Ignition.Models;
    using Ignition.Views;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
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
        public ReactiveCommand<Unit, Unit> Launch { get; }
        public ReactiveCommand<Unit, Unit> SettingsPanel { get; }

        public ReactiveCommand<string, Unit> ModNewsItem { get; }
        public ReactiveCommand<string, Unit> SiriusNewsItem { get; }

        [Reactive]
        public List<NewsItem> SiriusNews { get; set; }

        [Reactive]
        public List<NewsItem> ModNews { get; set; }

        [Reactive]
        public User User { get; set; }

        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        public LandingWindowViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Hangar = ReactiveCommand.Create(HangarButtonClick);
            Achievements = ReactiveCommand.Create(AchievementsButtonClick);
            Announcements = ReactiveCommand.Create(AnnouncementsButtonClick);
            Launch = ReactiveCommand.Create(SettingsPanelClick);
            SettingsPanel = ReactiveCommand.Create(LaunchButtonClick);
            ModNewsItem = ReactiveCommand.Create<string>(ModNewsClick);
            SiriusNewsItem = ReactiveCommand.Create<string>(SiriusNewsClick);

            SiriusNews = primaryWindowViewModel.SiriusNews;

            ModNews = primaryWindowViewModel.ModNews;

            User = primaryWindowViewModel.LoggedUser;
        }

        private void LaunchButtonClick()
        {
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
                DEVMODE devMode = default;
                devMode.dmSize = (short)Marshal.SizeOf(devMode);
                EnumDisplaySettings(null, -1, ref devMode);

                //Debug.WriteLine($"{devMode.dmPelsWidth} x {devMode.dmPelsHeight}");
            }
            else
            {
                //Debug.WriteLine($"{Settings.Instance.LauncherData.WidthResolution} x {Settings.Instance.LauncherData.HeightResolution}");
            }

            Process[] pname = Process.GetProcessesByName("freelancer");
            if (pname.Length == 0)
            {
                Process.Start(@"D:\Aftermath\EXE\Freelancer.exe", string.Join(" ", exeArguments.ToArray()));
            }
            else
            {
            }

            //TODO: Make sure that it pull the ID successfully or not launch
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