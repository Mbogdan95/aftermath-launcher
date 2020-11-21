namespace Ignition.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Ignition.Api;
    using Ignition.Models;
    using Ignition.Utils;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Reactive;
    using System.Reflection;
    using System.Threading.Tasks;

    public class PrimaryWindowViewModel : BaseViewModel
    {
        public string IgVersion => Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion).ToString();
        public string GameVersion { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }
        public ReactiveCommand<Unit, Unit> Minimize { get; }
        public ReactiveCommand<string, Unit> UpdateView { get; }

        [Reactive]
        public BaseViewModel SelectedViewModel { get; set; }

        public List<NewsItem> SiriusNews = new List<NewsItem>();
        public List<NewsItem> ModNews = new List<NewsItem>();

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
            SiriusNews = new List<NewsItem>();
            ModNews = new List<NewsItem>();

            using WebClient wc = new WebClient();

            var jsonString = wc.DownloadString(Settings.Instance.LauncherData.NewsLocation);

            foreach (JObject item in JsonConvert.DeserializeObject<JArray>(jsonString).ToObject<List<JObject>>())
            {
                NewsItem newsItem = new NewsItem() { Title = item["title"].ToString(), Description = "  " + item["subtitle"].ToString(), Date = item["date"].ToString(), Image = Utils.GetImageFromUrl(item["imageUrl"].ToString()), NewsUrl = item["url"].ToString() };

                SiriusNews.Add(newsItem);
                ModNews.Add(newsItem);
            }
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
