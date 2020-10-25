namespace Ignition.ViewModels
{
    using ReactiveUI;
    using System.Reactive;

    public class AchievementsViewModel : BaseViewModel
    {
        private PrimaryWindowViewModel primaryWindowViewModel;

        public ReactiveCommand<Unit, Unit> Back { get; }

        public AchievementsViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Back = ReactiveCommand.Create(GoBack);
        }

        private void GoBack()
        {
            primaryWindowViewModel.OnUpdateView("LandingPage");
        }
    }
}
