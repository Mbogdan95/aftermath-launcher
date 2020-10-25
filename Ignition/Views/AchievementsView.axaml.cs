namespace Ignition.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class AchievementsView : UserControl
    {
        public AchievementsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
