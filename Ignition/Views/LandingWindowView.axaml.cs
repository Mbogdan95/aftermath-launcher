namespace Ignition.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class LandingWindowView : UserControl
    {
        private Carousel carousel;
        private Button previousItemButton;
        private Button nextItemButton;

        public LandingWindowView()
        {
            InitializeComponent();

            previousItemButton.Click += (s, e) => carousel.Previous();
            nextItemButton.Click += (s, e) => carousel.Next();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            carousel = this.FindControl<Carousel>("carousel");
            previousItemButton = this.FindControl<Button>("PreviousItem");
            nextItemButton = this.FindControl<Button>("NextItem");
        }
    }
}
