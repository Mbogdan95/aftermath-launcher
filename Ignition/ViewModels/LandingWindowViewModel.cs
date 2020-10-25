namespace Ignition.ViewModels
{
    using Ignition.Models;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive;

    public class LandingWindowViewModel : BaseViewModel
    {
        private PrimaryWindowViewModel primaryWindowViewModel;

        public ReactiveCommand<Unit, Unit> Hangar { get; }
        public ReactiveCommand<Unit, Unit> Achievements { get; }
        public ReactiveCommand<Unit, Unit> Announcements { get; }
        public ReactiveCommand<Unit, Unit> Launch { get; }

        [Reactive]
        public List<NewsItem> RpNews { get; set; }

        [Reactive]
        public List<NewsItem> OorpNews { get; set; }

        public LandingWindowViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Hangar = ReactiveCommand.Create(HangarButtonClick);
            Achievements = ReactiveCommand.Create(AchievementsButtonClick);
            Announcements = ReactiveCommand.Create(AnnouncementsButtonClick);
            Launch = ReactiveCommand.Create(LaunchButtonClick);

            RpNews = primaryWindowViewModel.RpNews;

            OorpNews = primaryWindowViewModel.OorpNews;
        }

        private void LaunchButtonClick()
        {
        }

        private void AnnouncementsButtonClick()
        {
            Debug.WriteLine("Announcements Button Click");
        }

        private void AchievementsButtonClick()
        {
            primaryWindowViewModel.OnUpdateView("Achievements");
        }

        private void HangarButtonClick()
        {
            primaryWindowViewModel.OnUpdateView("Hangar");
        }
    }
}