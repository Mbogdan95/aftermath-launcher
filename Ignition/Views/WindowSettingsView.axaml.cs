using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ignition.Api;
using System;
using System.Text.RegularExpressions;

namespace Ignition.Views
{
    public class WindowSettingsView : UserControl
    {
        private TextBox widthResolutionTextBox;
        private TextBox heightResolutionTextBox;

        public WindowSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            widthResolutionTextBox = this.FindControl<TextBox>("WidthResolutionTextBox");
            heightResolutionTextBox = this.FindControl<TextBox>("HeightResolutionTextBox");

            this.FindControl<ToggleSwitch>("WindowedModeToggleSwitch").Click += ToggleSwitchClick;
            this.FindControl<ToggleSwitch>("DesktopResolutionToggleSwitch").Click += ToggleSwitchClick;

            this.FindControl<ToggleSwitch>("WindowedModeToggleSwitch").IsChecked = Settings.Instance.LauncherData.WindowedMode;
            this.FindControl<ToggleSwitch>("DesktopResolutionToggleSwitch").IsChecked = Settings.Instance.LauncherData.DefaultDesktopResolution;

            this.FindControl<TextBox>("WidthResolutionTextBox").KeyUp += TextBoxTextInput;
            this.FindControl<TextBox>("HeightResolutionTextBox").KeyUp += TextBoxTextInput;

            widthResolutionTextBox.Text = Settings.Instance.LauncherData.WidthResolution.ToString();
            heightResolutionTextBox.Text = Settings.Instance.LauncherData.HeightResolution.ToString();
        }

        private void TextBoxTextInput(object sender, Avalonia.Input.KeyEventArgs e)
        {
            string widthResolutionTextBoxText = widthResolutionTextBox.Text;
            string heightResolutionTextBoxText = heightResolutionTextBox.Text;

            Regex rgx = new Regex("[^0-9]");
            widthResolutionTextBoxText = rgx.Replace(widthResolutionTextBoxText, "");
            heightResolutionTextBoxText = rgx.Replace(heightResolutionTextBoxText, "");

            Settings.Instance.SetResolution(Convert.ToInt32(widthResolutionTextBoxText), Convert.ToInt32(heightResolutionTextBoxText));
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
