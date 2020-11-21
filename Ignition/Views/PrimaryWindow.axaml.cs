namespace Ignition.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Ignition.ViewModels;

    public class PrimaryWindow : Window
    {
        public PrimaryWindow()
        {
            InitializeComponent();

            DataContext = new PrimaryWindowViewModel();
            HasSystemDecorations = false;
            CanResize = false;

            //Program.Selector.EnableThemes(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
