using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ignition.Api;
using System;

namespace Ignition.Views
{
    public class WindowSettingsView : UserControl
    {
        public WindowSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<ToggleSwitch>("WindowedModeToggleSwitch").Click += ToggleSwitchClick;
            this.FindControl<ToggleSwitch>("DesktopResolutionToggleSwitch").Click += ToggleSwitchClick;

            this.FindControl<ToggleSwitch>("WindowedModeToggleSwitch").IsChecked = Settings.Instance.LauncherData.WindowedMode;
            this.FindControl<ToggleSwitch>("DesktopResolutionToggleSwitch").IsChecked = Settings.Instance.LauncherData.DefaultDesktopResolution;

            this.FindControl<TextBox>("WidthResolutionTextBox").KeyUp += TextBoxTextInput;
            this.FindControl<TextBox>("HeightResolutionTextBox").KeyUp += TextBoxTextInput;

            this.FindControl<TextBox>("WidthResolutionTextBox").Text = Settings.Instance.LauncherData.WidthResolution.ToString();
            this.FindControl<TextBox>("HeightResolutionTextBox").Text = Settings.Instance.LauncherData.HeightResolution.ToString();
        }

        private void TextBoxTextInput(object sender, Avalonia.Input.KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Name == "WidthResolutionTextBox")
            {
                Settings.Instance.SetWidthResolution(Convert.ToInt32(textBox.Text));
            }
            else if (textBox.Name == "HeightResolutionTextBox")
            {
                Settings.Instance.SetHeightResolution(Convert.ToInt32(textBox.Text));
            }
        }

        private void ToggleSwitchClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

            if (toggleSwitch.Name == "WindowedModeToggleSwitch")
            {
                Settings.Instance.SetWindowedMode((bool)toggleSwitch.IsChecked);
            }
            else if (toggleSwitch.Name == "DesktopResolutionToggleSwitch")
            {
                Settings.Instance.SetDefaultDesktopResolution((bool)toggleSwitch.IsChecked);
            }
        }
    }
}
