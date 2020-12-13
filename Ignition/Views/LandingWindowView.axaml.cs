namespace Ignition.Views
{
    using Avalonia.Animation;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using System;

    public class LandingWindowView : UserControl
    {
        private Carousel carousel;
        private Button previousItemButton;
        private Button nextItemButton;

        private DispatcherTimer timer;

        public LandingWindowView()
        {
            InitializeComponent();

            previousItemButton.Click += (s, e) => PreviousCarouselItem();
            nextItemButton.Click += (s, e) => NextCarouselItem(true);

            carousel.PageTransition = new CrossFade(TimeSpan.FromSeconds(0.25));

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += (s, e) => NextCarouselItem(false);
            timer.Start();
        }

        private void NextCarouselItem(bool clickedByUser)
        {
            if (clickedByUser)
            {
                timer.Stop();
                timer.Start();
            }

            if (carousel.SelectedIndex == carousel.ItemCount - 1)
            {
                carousel.SelectedIndex = 0;
            }
            else
            {
                carousel.Next();
            }
        }

        private void PreviousCarouselItem()
        {
            if (carousel.SelectedIndex == 0)
            {
                carousel.SelectedIndex = carousel.ItemCount - 1;
            }
            else
            {
                carousel.Previous();
            }
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
