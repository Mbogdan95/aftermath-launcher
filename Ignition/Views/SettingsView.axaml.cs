using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ignition.Views
{
    public class SettingsView : UserControl
    {
        public SettingsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
