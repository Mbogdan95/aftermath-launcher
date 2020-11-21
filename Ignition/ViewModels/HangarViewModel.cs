namespace Ignition.ViewModels
{
    using Ignition.Models;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System.Collections.Generic;
    using System.Reactive;

    public class HangarViewModel : BaseViewModel
    {
        private PrimaryWindowViewModel primaryWindowViewModel;

        private List<Ship> ships = new List<Ship>();

        private Ship selectedShip = null;

        [Reactive]
        public string Ship1Image { get; set; }
        [Reactive]
        public string Ship2Image { get; set; }
        [Reactive]
        public string Ship3Image { get; set; }
        [Reactive]
        public string Ship4Image { get; set; }
        [Reactive]
        public string Ship5Image { get; set; }

        [Reactive]
        public bool IsShip1Selected { get; set; }
        [Reactive]
        public bool IsShip2Selected { get; set; }
        [Reactive]
        public bool IsShip3Selected { get; set; }
        [Reactive]
        public bool IsShip4Selected { get; set; }
        [Reactive]
        public bool IsShip5Selected { get; set; }

        [Reactive]
        public string Location { get; set; }
        [Reactive]
        public string Base { get; set; }
        [Reactive]
        public string Affiliation { get; set; }
        [Reactive]
        public string Cargo { get; set; }
        [Reactive]
        public string SelectedShipImage { get; set; }

        public ReactiveCommand<Unit, Unit> Back { get; }
        public ReactiveCommand<Unit, Unit> Left { get; }
        public ReactiveCommand<Unit, Unit> Right { get; }

        public HangarViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Back = ReactiveCommand.Create(GoBack);

            ships = primaryWindowViewModel.LoggedUser.Ships;

            SetImageForShips();

            SelectShip1();
        }

        public void SelectShip1()
        {
            if (ships.Count > 0)
            {
                selectedShip = ships[0];

                SetStringsValue();

                IsShip1Selected = true;

                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
            else
            {
                selectedShip = null;

                SetStringsValue();

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
        }

        public void SelectShip2()
        {
            if (ships.Count > 1)
            {
                selectedShip = ships[1];

                SetStringsValue();

                IsShip2Selected = true;

                IsShip1Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
            else
            {
                selectedShip = null;

                SetStringsValue();

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
        }

        public void SelectShip3()
        {
            if (ships.Count > 2)
            {
                selectedShip = ships[2];

                SetStringsValue();

                IsShip3Selected = true;

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
            else
            {
                selectedShip = null;

                SetStringsValue();

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
        }

        public void SelectShip4()
        {
            if (ships.Count > 3)
            {
                selectedShip = ships[3];

                SetStringsValue();

                IsShip4Selected = true;

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip5Selected = false;
            }
            else
            {
                selectedShip = null;

                SetStringsValue();

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
        }

        public void SelectShip5()
        {
            if (ships.Count > 4)
            {
                selectedShip = ships[4];

                SetStringsValue();

                IsShip5Selected = true;

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
            }
            else
            {
                selectedShip = null;

                SetStringsValue();

                IsShip1Selected = false;
                IsShip2Selected = false;
                IsShip3Selected = false;
                IsShip4Selected = false;
                IsShip5Selected = false;
            }
        }

        private void GoBack()
        {
            primaryWindowViewModel.OnUpdateView("LandingPage");
        }

        private void SetStringsValue()
        {
            if (selectedShip != null)
            {
                Location = $"Location: {selectedShip.Location}";
                Base = $"Base: {selectedShip.Base}";
                Affiliation = $"Affiliation: {selectedShip.Affiliation}";
                Cargo = $"Cargo: {selectedShip.Cargo}";
                SelectedShipImage = $"{selectedShip.ShipName}.png";
            }
            else
            {
                Location = string.Empty;
                Base = string.Empty;
                Affiliation = string.Empty;
                Cargo = string.Empty;
                SelectedShipImage = string.Empty;
            }
        }

        private void SetImageForShips()
        {
            for (int i = 0; i < ships.Count; i++)
            {
                if (i == 0)
                {
                    Ship1Image = $"{ships[i].ShipName}.png";
                }
                else if (i == 1)
                {
                    Ship2Image = $"{ships[i].ShipName}.png";
                }
                else if (i == 2)
                {
                    Ship3Image = $"{ships[i].ShipName}.png";
                }
                else if (i == 3)
                {
                    Ship4Image = $"{ships[i].ShipName}.png";
                }
                else if (i == 4)
                {
                    Ship5Image = $"{ships[i].ShipName}.png";
                }
            }
        }
    }
}
