namespace Ignition.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
