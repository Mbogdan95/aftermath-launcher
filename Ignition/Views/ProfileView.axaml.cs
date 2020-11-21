namespace Ignition.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
