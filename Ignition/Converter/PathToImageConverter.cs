namespace Ignition.Converter
{
    using Avalonia;
    using Avalonia.Data.Converters;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;

    public class PathToImageConverter : IValueConverter
    {
        public static PathToImageConverter Instance = new PathToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            else if (string.IsNullOrEmpty((string)value))
            {
                return null;
            }

            if (value is string rawUri && targetType == typeof(IBitmap))
            {
                Uri uri;

                if (Uri.TryCreate(rawUri, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    WebClient wc = new WebClient();
                    byte[] originalData = wc.DownloadData(uri);

                    MemoryStream stream = new MemoryStream(originalData);

                    return new Bitmap(stream);
                }
                else
                {
                    // Allow for assembly overrides
                    if (rawUri.StartsWith("avares://"))
                    {
                        uri = new Uri(rawUri);
                    }
                    else
                    {
                        string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                        uri = new Uri($"avares://{assemblyName}/Assets/{rawUri}");
                    }

                    var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    var asset = assets.Open(uri);

                    return new Bitmap(asset);
                }
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
