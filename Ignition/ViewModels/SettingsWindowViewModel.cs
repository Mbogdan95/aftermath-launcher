using ReactiveUI;
using System;
using System.Reactive;

namespace Ignition.ViewModels
{
    public class SettingsWindowViewModel : BaseViewModel
    {
        public ReactiveCommand<Unit, Unit> Close { get; }

        public SettingsWindowViewModel()
        {
            Close = ReactiveCommand.Create(CloseButtonClick);
        }

        private void CloseButtonClick()
        {
            Environment.Exit(0);
        }
    }
}
