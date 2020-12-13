using Avalonia.Controls;
using Ignition.Api;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ignition.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        private PrimaryWindowViewModel primaryWindowViewModel;

        public LoadingViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            GetGameVersion();
        }
        
        // TODO: SPLASH SCREEN: make it look pretty (Laz's words)

        /// <summary>
        /// Load news
        /// </summary>
        /// <returns>Succeeded or no</returns>
        public async Task<bool> LoadNews()
        {
            // Get news from API
            (bool, List<NewsLoader.NewsItem>, List<NewsLoader.NewsItem>) allNews = await NewsLoader.LoadNews();

            // Check if news were loaded
            if (!allNews.Item1)
            {
                // Log error
                Logger.WriteLog("News were not loaded");

                // Show message box that launcher was unable to load news
                await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
                {
                    Style = Style.UbuntuLinux,
                    ContentMessage = "Unable to load data from server. Check connection and try again later. Application will now terminate. \n",
                    ContentHeader = "ERROR",
                    Icon = Icon.Error,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ShowInCenter = false,
                    ButtonDefinitions = new[]
                {
                    new ButtonDefinition { Name = "OK", Type = ButtonType.Colored }
                }
                }).ShowDialog(primaryWindowViewModel.Desktop.MainWindow);

                return false;
            }

            // Store news
            primaryWindowViewModel.SiriusNews = allNews.Item2;
            primaryWindowViewModel.ModNews = allNews.Item3;

            return true;
        }

        /// <summary>
        /// Change the view 
        /// </summary>
        public void ChangeView()
        {
            // Change view to LoginPage
            primaryWindowViewModel.OnUpdateView("LoginPage");
        }

        /// <summary>
        /// Gets the current game version
        /// </summary>
        private void GetGameVersion()
        {
            try
            {
                // Get info of Freelancer.exe
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(Settings.Instance.LauncherData.AftermathInstall + "/EXE/Freelancer.exe");

                // Split the file version in a string array
                string[] arr = info.FileVersion.Split(", ");

                // Set the game version
                primaryWindowViewModel.GameVersion = $"{arr[0]}.{arr[1]}.{arr[2]}";

                // Set the game as installed
                primaryWindowViewModel.GameInstalled = true;
            }
            catch
            {
                // No game version
                primaryWindowViewModel.GameVersion = "N/A";

                // Game not installed
                primaryWindowViewModel.GameInstalled = false;
            }
        }
    }
}
