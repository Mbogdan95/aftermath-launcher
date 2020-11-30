namespace Ignition.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Media.Imaging;
    using Ignition.Api;
    using Ignition.Models;
    using Ignition.Views;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reactive;
    using System.Reflection;
    using System.Threading.Tasks;
    using WebRequest = Api.WebRequest;


    public class PrimaryWindowViewModel : BaseViewModel
    {
        // Variable that stores the launcher version
        public string IgVersion => Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion).ToString();

        // Variable that stores the game version
        [Reactive]
        public string GameVersion { get; set; }

        // Variable that stores the state of the game INSTALLED/NOT INSTALLED
        [Reactive]
        public bool GameInstalled { get; set; }

        // Variables for buttons
        public ReactiveCommand<Unit, Unit> Close { get; }
        public ReactiveCommand<Unit, Unit> Minimize { get; }
        public ReactiveCommand<string, Unit> UpdateView { get; }
        public ReactiveCommand<Unit, Unit> SettingsPanel { get; }

        [Reactive]
        public BaseViewModel SelectedViewModel { get; set; }

        // Variables that store the news
        public List<NewsItem> SiriusNews = new List<NewsItem>();
        public List<NewsItem> ModNews = new List<NewsItem>();

        // Variable that stores the logged user
        public User LoggedUser = new User();

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryWindowViewModel"/> class.
        /// </summary>
        public PrimaryWindowViewModel()
        {
            // Set the login view model as the current view model
            SelectedViewModel = new LoginViewModel(this);

            // Set methods to buttons
            Close = ReactiveCommand.Create(CloseButtonClick);
            Minimize = ReactiveCommand.Create(MinimizeButtonClick);
            UpdateView = ReactiveCommand.Create<string>(OnUpdateView);
            SettingsPanel = ReactiveCommand.Create(SettingsPanelClick);

            // Get the game version
            UpdateGameVersion();

            // Load news
            LoadNews();
        }

        /// <summary>
        /// Gets the current game version
        /// </summary>
        private void UpdateGameVersion()
        {
            try
            {
                // Get info of Freelancer.exe
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(Settings.Instance.LauncherData.AftermathInstall + "/EXE/Freelancer.exe");

                // Split the file version in a string array
                string[] arr = info.FileVersion.Split(", ");

                // Set the game version
                GameVersion = $"{arr[0]}.{arr[1]}.{arr[2]}";

                // Set the game as installed
                GameInstalled = true;
            }
            catch
            {
                // No game version
                GameVersion = "N/A";

                // Game not installed
                GameInstalled = false;
            }
        }

        /// <summary>
        /// Method to change the current selected view model
        /// </summary>
        /// <param name="parameter">Name of the view model to be selected</param>
        public void OnUpdateView(string parameter)
        {
            // Check the parameter value
            if (parameter == "LandingPage")
            {
                // Set landing window view model
                SelectedViewModel = new LandingWindowViewModel(this);
            }
            else if (parameter == "Hangar")
            {
                // Set hangar view model
                SelectedViewModel = new HangarViewModel(this);
            }
            else if (parameter == "Achievements")
            {
                // Set achievements view model
                SelectedViewModel = new AchievementsViewModel(this);
            }
        }

        /// <summary>
        /// Load news from URL
        /// </summary>
        private async Task LoadNews()
        {
            try
            {
                // Initialize variables
                SiriusNews = new List<NewsItem>();
                ModNews = new List<NewsItem>();

                using WebClient webClient = new WebClient();

                // Get JSON string from link
                string jsonString = webClient.DownloadString(Settings.Instance.LauncherData.NewsLocation);

                var blogs = await WebRequest.GetRequest("/api/blog?page=1&count=3");

                foreach (var item in blogs.Value)
                {
                    byte[] bytes = Convert.FromBase64String(item["Banner"]["data"].ToString());

                    Bitmap image;
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        image = new Bitmap(ms);
                    }

                    NewsItem newsItem = new NewsItem()
                    {
                        Title = item["Title"].ToString(),
                        Description = item["Description"].ToString(),
                        Date = DateTime.Parse(item["CreationDate"].ToString()).ToString(),
                        Image = image,
                        NewsUrl = $"https://forums.aftermath.space/blogs/{item["Id"]}"
                    };

                    ModNews.Add(newsItem);
                }

                // Loop through each JObject
                foreach (JObject item in JsonConvert.DeserializeObject<JArray>(jsonString).ToObject<List<JObject>>())
                {
                    // Create news item
                    NewsItem newsItem = new NewsItem() { Title = item["title"].ToString(), Description = "  " + item["subtitle"].ToString(), Date = item["date"].ToString(), Image = WebRequest.GetImageFromUrl(item["imageUrl"].ToString()), NewsUrl = item["url"].ToString() };

                    SiriusNews.Add(newsItem);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Error occurred while loading news. Error: " + ex.Message);
            }
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
                settingsWindow.ShowDialog((Window)singleView.MainView);
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
