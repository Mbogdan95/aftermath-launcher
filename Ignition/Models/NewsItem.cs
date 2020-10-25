namespace Ignition.Models
{
    using Avalonia.Media.Imaging;

    public class NewsItem
    {
        public string Title { get; set; }

        public string Date { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string NewsUrl { get; set; }

        public Bitmap Image { get; set; }
    }
}
