﻿namespace Ignition.Models
{
    using Avalonia.Media.Imaging;
    using System.Collections.Generic;

    public class User
    {
        public string Username { get; set; }

        public string Credits { get; set; }

        public string WarningLevel { get; set; }

        public string PlayerID { get; set; }

        public string Rank { get; set; }

        public int Level { get; set; }

        public int LevelProgress { get; set; }

        public uint AccCode { get; set; }

        public uint AccSig { get; set; }

        public List<Ship> Ships { get; set; }

        public Bitmap Avatar { get; set; }
    }
}
