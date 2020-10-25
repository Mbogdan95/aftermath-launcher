namespace Ignition.ViewModels
{
    using Avalonia.Media;
    using Ignition.Models;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Reactive;

    public class LoginViewModel : BaseViewModel
    {
        public ReactiveCommand<Unit, Unit> Login { get; }
        public ReactiveCommand<Unit, Unit> ForgottenPassword { get; }
        public ReactiveCommand<Unit, Unit> NeedAnAccount { get; }

        private PrimaryWindowViewModel primaryWindowViewModel;

        [Reactive]
        public string Email { get; set; }

        [Reactive]
        public string Password { get; set; }

        public string LoginError { get; set; }

        public SolidColorBrush LoginErrorColor { get; set; }

        public LoginViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Login = ReactiveCommand.Create(LoginButtonClick);
            ForgottenPassword = ReactiveCommand.Create(ForgottenPasswordButtonClick);
            NeedAnAccount = ReactiveCommand.Create(NeedAnAccountButtonClick);
        }

        private void LoginButtonClick()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                LoginError = "Email field is empty";
                LoginErrorColor = new SolidColorBrush(Colors.Red);

                this.RaisePropertyChanged(nameof(LoginErrorColor));
                this.RaisePropertyChanged(nameof(LoginError));

                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                LoginError = "Password field is empty";
                LoginErrorColor = new SolidColorBrush(Colors.Red);

                this.RaisePropertyChanged(nameof(LoginErrorColor));
                this.RaisePropertyChanged(nameof(LoginError));

                return;
            }

            if (Email == "root" && Password == "root")
            {
                User loggedUser = new User()
                {
                    Username = "Nepo",
                    Credits = "2,982,943,889",
                    WarningLevel = "110%",
                    Level = 9000,
                    Rank = "SNAC Lover",
                    Ships = new List<Ship>()
                    {
                         new Ship() { ShipName = "Li_elite2", Location = "New York", Base = "Planet Manhatan", Affiliation = "Liberty Navy", Cargo = "Water" },
                         new Ship() { ShipName = "Bw_vheavy_fighter", Location = "Omicron Alpha", Base = "Planet Malta", Affiliation = "Outcasts", Cargo = "Cardamine, Pilots" },
                         new Ship() { ShipName = "Havoc_mk_II", Location = "New London", Base = "Planet Malta", Affiliation = "Outcasts", Cargo = "SNACs" },
                         new Ship() { ShipName = "Cv_vheavy_fighter", Location = "Omicron Alpha", Base = "Planet Malta", Affiliation = "Outcasts", Cargo = "Empty" },
                         new Ship() { ShipName = "Br_elite", Location = "Omicron Alpha", Base = "Planet Malta", Affiliation = "Outcasts", Cargo = "Credit cards, Food rations, Oxygen, Water" }
                    }
                };

                primaryWindowViewModel.LoggedUser = loggedUser;


                HideLoginWindow();
            }
            else
            {
                LoginError = "Email and password do not match";
                LoginErrorColor = new SolidColorBrush(Colors.Red);

                this.RaisePropertyChanged(nameof(LoginErrorColor));
                this.RaisePropertyChanged(nameof(LoginError));

                return;
            }
        }

        private void HideLoginWindow()
        {
            primaryWindowViewModel.OnUpdateView("LandingPage");
        }

        private void NeedAnAccountButtonClick()
        {
            throw new NotImplementedException();
        }

        private void ForgottenPasswordButtonClick()
        {
            throw new NotImplementedException();
        }
    }
}
