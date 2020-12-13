namespace Ignition.Views
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Ignition.ViewModels;

    public class PrimaryWindow : Window
    {
        public PrimaryWindow()
        {
            InitializeComponent();

            DataContext = new PrimaryWindowViewModel((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime);

            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
