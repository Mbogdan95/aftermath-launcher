namespace Ignition.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Ignition.Models;
    using Ignition.Utils;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive;
    using System.Reflection;
    using System.Threading.Tasks;

    public class PrimaryWindowViewModel : BaseViewModel
    {
        public string IgVersion => Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion).ToString();
        public string GameVersion { get; private set; }

        public ReactiveCommand<Unit, Unit> Close { get; }
        public ReactiveCommand<Unit, Unit> Minimize { get; }
        public ReactiveCommand<string, Unit> UpdateView { get; }

        [Reactive]
        public BaseViewModel SelectedViewModel { get; set; }

        public List<NewsItem> RpNews = new List<NewsItem>();
        public List<NewsItem> OorpNews = new List<NewsItem>();

        public User LoggedUser = new User();

        public PrimaryWindowViewModel()
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(@"D:\Aftermath\EXE\Freelancer.exe");
            string[] arr = info.FileVersion.Split(", ");
            GameVersion = $"{arr[0]}.{arr[1]}.{arr[2]}";

            Close = ReactiveCommand.Create(CloseButtonClick);
            Minimize = ReactiveCommand.Create(MinimizeButtonClick);
            UpdateView = ReactiveCommand.Create<string>(OnUpdateView);

            SelectedViewModel = new LoginViewModel(this);

            Task.Factory.StartNew(() => LoadNews());
        }

        public void OnUpdateView(string parameter)
        {
            if (parameter == "LandingPage")
            {
                SelectedViewModel = new LandingWindowViewModel(this);
            }
            else if (parameter == "Hangar")
            {
                SelectedViewModel = new HangarViewModel(this);
            }
            else if (parameter == "Achievements")
            {
                SelectedViewModel = new AchievementsViewModel(this);
            }
        }

        private void LoadNews()
        {
            RpNews = new List<NewsItem>()
            {
                new NewsItem() { Description = "Gallia blasts the shit out of Planet Leeds, killing billions", Date = "DATE TEST", Title = "PLANET LEEDS UNDER HEAVY BOMBARDMENT" },
                new NewsItem() { Description = "They dumb as fuck and kill eachother", Date = "September 1st 815 A.S.", Title = "RHENINALD CIVIL WAR" },
                new NewsItem() { Description = "Devtonia, sorry, Bretonia takes by forece another system not givin a shit", Date = "September 2nd 815 A.S", Title = "BRETONIA TAKES BY FORCE ANOTHER SYSTEM" },
                new NewsItem() { Description = "Who knew Cardamine can be used in the state of plasma", Date = "September 3rd 815 A.S", Title = "NEW FORM OF CARDAMINE DISCOVERED: PLASMA CARDAMINE" },
                new NewsItem() { Description = "Deep in the Omicrons Nomads have been spotted once more. No one know what to do. Everyone asks where is our lord saviour Edison Trent", Date = "September 5th 815 A.S", Title = "NOMADS ARE BACK. SIRIUS PANICS. WHERE IS TRENT?" }
            };

            OorpNews = new List<NewsItem>()
            {
                new NewsItem()
                {
                    Description = "Equipment: \n" +
                "- Added 'Supernova Antimmater Cannon' as bomber weapon (Tribute to Nepo's hard work)", Date = DateTime.Today.ToString(), Title = "PATCH NOTES", Image = Utils.GetImageFromUrl("https://cdn.discordapp.com/attachments/753297236896514130/753297259755471038/unknown.png")
                },
                new NewsItem() { Description = "From now on every word, discussion or talk about Discovery will get you banned. We use some super duper AI shit like in the movies to detect this", Date = DateTime.Today.ToString(), Title = "NEW WORDS ADDED TO FILTER", Image = Utils.GetImageFromUrl("https://i.imgur.com/wremWlr.jpg") },
                new NewsItem() { Description = "6.9 - Only furries and anime girls characters are allowed, eveything else will get you banned", Date = DateTime.Today.ToString(), Title = "RULES CHANGE: 6.9" },
                new NewsItem() { Description = "YES, Nepo is awesome", Date = DateTime.Today.ToString(), Title = "NEPO IS AWESOME" }
            };
        }

        private void CloseButtonClick()
        {
            Environment.Exit(0);
        }

        private void MinimizeButtonClick()
        {
            Window primaryWindow = new Window();

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                primaryWindow = desktop.MainWindow;
            }
            else if (Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                primaryWindow = (Window)singleView.MainView;
            }

            primaryWindow.WindowState = WindowState.Minimized;
        }
    }
}
