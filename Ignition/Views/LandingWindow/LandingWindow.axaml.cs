using Avalonia.Styling;

namespace Ignition.Views.LandingWindow
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Ignition.ViewModels.LandingWindow;

    public class LandingWindow : UserControl
    {
        public LandingWindow()
        {
            InitializeComponent();
            this.DataContext = new LandingWindowViewModel();
            this.Classes = new Classes("View");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
