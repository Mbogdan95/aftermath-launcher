namespace Ignition.Views
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Ignition.ViewModels;

    public class PrimaryWindow : Window
    {
        public PrimaryWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = new PrimaryWindowViewModel();
            HasSystemDecorations = false;
            CanResize = false;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
