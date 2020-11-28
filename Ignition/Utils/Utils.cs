using System.Linq;

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
    using System.Net.Http.Headers;
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

        private enum RequestType
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH
        }

        private static async Task<KeyValuePair<HttpStatusCode, JObject>> Request(string url, object obj, RequestType type)
        {
            string rootUrl = Settings.Instance.LauncherData.ApiLocation;
            HttpClient client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(Settings.Instance.LauncherData.Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Instance.LauncherData.Token);
            }

            StringContent stringContent = obj is null ? new StringContent("{}", Encoding.UTF8, "application/json") : new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            HttpResponseMessage msg = null;
            switch (type)
            {
                case RequestType.GET:
                    msg = await client.GetAsync(rootUrl + url);
                    break;
                case RequestType.POST:
                    msg = await client.PostAsync(rootUrl + url, stringContent);
                    break;
                case RequestType.PUT:
                    msg = await client.PutAsync(rootUrl + url, stringContent);
                    break;
                case RequestType.DELETE:
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(rootUrl + url),
                        Content = stringContent
                    };
                    msg = await client.SendAsync(request);
                    break;
                case RequestType.PATCH:
                    msg = await client.PatchAsync(rootUrl + url, stringContent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var key = msg.StatusCode;
            var value = await msg.Content.ReadAsStringAsync();

            var retObj = JsonConvert.DeserializeObject<JObject>(value);
            var errors = retObj["errors"].Cast<List<string>>();
            if (key != HttpStatusCode.Accepted && key == HttpStatusCode.OK && key != HttpStatusCode.Created)
            {
                // TODO: show big angry error
            }

            return new KeyValuePair<HttpStatusCode, JObject>(key, retObj["result"].ToObject<JObject>());
        }

        public static async Task<KeyValuePair<HttpStatusCode, JObject>> GetRequest(string url) => await Request(url, null, RequestType.GET);
        public static async Task<KeyValuePair<HttpStatusCode, JObject>> PutRequest(string url, object obj) => await Request(url, obj, RequestType.PUT);
        public static async Task<KeyValuePair<HttpStatusCode, JObject>> DeleteRequest(string url, object obj) => await Request(url, obj, RequestType.DELETE);
        public static async Task<KeyValuePair<HttpStatusCode, JObject>> PatchRequest(string url, object obj) => await Request(url, obj, RequestType.PATCH);
        public static async Task<KeyValuePair<HttpStatusCode, JObject>> PostRequest(string url, object obj) => await Request(url, obj, RequestType.POST);
    }
}
