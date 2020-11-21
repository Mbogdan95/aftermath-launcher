namespace Ignition.Utils
{
    using Avalonia.Media.Imaging;
    using Ignition.Api;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public static class Utils
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

        public static async Task<KeyValuePair<HttpStatusCode, JObject>> PutRequest(string url, object obj)
        {
            string rootUrl = Settings.Instance.LauncherData.ApiLocation;

            HttpClient client = new HttpClient();

            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            var result = await client.PutAsync(rootUrl + url, stringContent);

            var key = result.StatusCode;
            var value = await result.Content.ReadAsStringAsync();

            return new KeyValuePair<HttpStatusCode, JObject>(key, JsonConvert.DeserializeObject<JObject>(value));
        }
    }
}
