namespace Ignition.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Ignition.Models;
    using Ignition.Views;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive;
    using System.Reflection;
    using static Ignition.Api.NewsLoader;

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

        // Variable that stores the logged user
        public User LoggedUser = new User();

        public List<NewsItem> SiriusNews;
        public List<NewsItem> ModNews;

        public IClassicDesktopStyleApplicationLifetime Desktop;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryWindowViewModel"/> class.
        /// </summary>
        public PrimaryWindowViewModel(IClassicDesktopStyleApplicationLifetime desktop)
        {
            Desktop = desktop;

            // Set the login view model as the current view model
            SelectedViewModel = new LoadingViewModel(this);

            // Set methods to buttons
            Close = ReactiveCommand.Create(CloseButtonClick);
            Minimize = ReactiveCommand.Create(MinimizeButtonClick);
            UpdateView = ReactiveCommand.Create<string>(OnUpdateView);
            SettingsPanel = ReactiveCommand.Create(SettingsPanelClick);
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
            else if (parameter == "LoginPage")
            {
                SelectedViewModel = new LoginViewModel(this);
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
