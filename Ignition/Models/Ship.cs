using Avalonia.Media.Imaging;

namespace Ignition.Models
{
    public class Ship
    {
        public string ShipName { get; set; }

        public string Location { get; set; }

        public string Base { get; set; }

        public string Affiliation { get; set; }

        public string Cargo { get; set; }

        public string Owner { get; set; }

        public string ImageUrl { get; set; }

        public Bitmap Image { get; set; }
    }
}
