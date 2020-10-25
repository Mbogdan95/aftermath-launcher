namespace Ignition.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class HangarView : UserControl
    {
        public HangarView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
