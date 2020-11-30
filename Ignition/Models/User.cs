namespace Ignition.Models
{
    using Avalonia.Media.Imaging;
    using System.Collections.Generic;

    public class User
    {
        public string Username { get; set; }

        public string Credits { get; set; }

        public string WarningLevel { get; set; }

        public long PlayerID { get; set; }

        public string Rank { get; set; }

        public int Level { get; set; }

        public int LevelProgress { get; set; }

        public string AccCode { get; set; }

        public string AccSig { get; set; }

        public List<Ship> Ships { get; set; }

        public Bitmap Avatar { get; set; }
    }
}
