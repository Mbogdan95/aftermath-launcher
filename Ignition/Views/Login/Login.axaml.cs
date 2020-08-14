using System;

namespace Ignition.Views.Login
{
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;

    public class LoginWindow : UserControl
    {
        public LoginWindow()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoginButtonClick(object sender, RoutedEventArgs args)
        {
            TextBox email = this.FindControl<TextBox>("EmailInput");
            TextBox password = this.FindControl<TextBox>("PasswordInput");
            Console.Beep();
        }
    }
}
