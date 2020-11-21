using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Ignition.Api;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ignition.Views
{
    public class LauncherSettingsView : UserControl
    {
        public LauncherSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<ToggleSwitch>("StayLoggedInToggleSwitch").Click += ToggleSwitchClick;
            this.FindControl<ToggleSwitch>("StayLoggedInToggleSwitch").IsChecked = Settings.Instance.LauncherData.StayLoggedIn;
        }

        private void ToggleSwitchClick(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

            if (toggleSwitch.Name == "StayLoggedInToggleSwitch")
            {
                Settings.Instance.SetStayLoggedIn((bool)toggleSwitch.IsChecked);
            }
        }

        private void ResetLauncherData(object sender, RoutedEventArgs e)
        {
            ShowMessageBox();
        }

        private async Task ShowMessageBox()
        {
            var messageBoxCustomWindow = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
            {
                Style = Style.Windows,
                ContentMessage = "All launcher data will be lost. Do you want to continue? NOTE: Launcher will restart automatically \n",
                ContentTitle = "WARNING",
                Icon = Icon.Warning,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowInCenter = true,
                ButtonDefinitions = new[]
                {
                    new ButtonDefinition {Name = "Cancel"},
                    new ButtonDefinition {Name = "OK", Type = ButtonType.Colored}
                }
            }).Show();

            if (messageBoxCustomWindow == "OK")
            {
                RestartProgram();
            }
        }

        private void RestartProgram()
        {
            var filePath = Process.GetCurrentProcess().MainModule.FileName;

            Process.Start(filePath);

            Environment.Exit(0);
        }
    }
}
