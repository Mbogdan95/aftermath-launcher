using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ignition.Views
{
    public class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
