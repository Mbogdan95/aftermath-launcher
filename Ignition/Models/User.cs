namespace Ignition.Models
{
    using System.Collections.Generic;

    public class User
    {
        public string Username { get; set; }

        public string Credits { get; set; }

        public string WarningLevel { get; set; }

        public int Level { get; set; }

        public string Rank { get; set; }

        public int LevelProgress { get; set; }

        public List<Ship> Ships { get; set; }
    }
}
