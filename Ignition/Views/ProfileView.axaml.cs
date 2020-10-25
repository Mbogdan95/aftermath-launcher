namespace Ignition.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Ignition.ViewModels;

    public class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
            DataContext = new ProfileViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
