using Avalonia.Media.Imaging;
using System;

namespace Ignition.Models
{
    public class AchievementItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool Unlocked { get; set; }

        public DateTime UnlockedAt { get; set; }

        public Bitmap Image { get; set; }
    }
}
