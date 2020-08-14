namespace Ignition.Views.LandingWindow
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Ignition.ViewModels.LandingWindow;

    public class ProfileView : UserControl
    {
        public ProfileView()
        {
            this.InitializeComponent();
            this.DataContext = new ProfileViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
