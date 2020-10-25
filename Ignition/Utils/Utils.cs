namespace Ignition.Utils
{
    using Avalonia.Media.Imaging;
    using System;
    using System.IO;
    using System.Net;

    public class Utils
    {
        public static Bitmap GetImageFromUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                WebClient wc = new WebClient();
                byte[] originalData = wc.DownloadData(uri);

                MemoryStream stream = new MemoryStream(originalData);

                return new Bitmap(stream);
            }
            else
            {
                return null;
            }
        }
    }
}
