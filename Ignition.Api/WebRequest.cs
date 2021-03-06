namespace Ignition.Api
{
    using Avalonia.Media.Imaging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public static class WebRequest
    {
        public static async Task<KeyValuePair<HttpStatusCode, JToken>> GetRequest(string url) => await Request(url, null, RequestType.GET);
        public static async Task<KeyValuePair<HttpStatusCode, JToken>> PutRequest(string url, object obj) => await Request(url, obj, RequestType.PUT);
        public static async Task<KeyValuePair<HttpStatusCode, JToken>> DeleteRequest(string url, object obj) => await Request(url, obj, RequestType.DELETE);
        public static async Task<KeyValuePair<HttpStatusCode, JToken>> PatchRequest(string url, object obj) => await Request(url, obj, RequestType.PATCH);
        public static async Task<KeyValuePair<HttpStatusCode, JToken>> PostRequest(string url, object obj) => await Request(url, obj, RequestType.POST);

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

        private static async Task<KeyValuePair<HttpStatusCode, JToken>> Request(string url, object obj, RequestType type)
        {
            string rootUrl = Settings.Instance.LauncherData.ApiLocation;

            HttpClient client = new HttpClient();

            if (!string.IsNullOrWhiteSpace(Settings.Instance.LauncherData.Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Instance.LauncherData.Token);
            }

            StringContent stringContent = obj is null ? new StringContent("{}", Encoding.UTF8, "application/json") : new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            try
            {
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

                if (retObj.ContainsKey("errors"))
                {
                    var errors = retObj["errors"].Cast<List<string>>();

                    if (key != HttpStatusCode.Accepted && key == HttpStatusCode.OK && key != HttpStatusCode.Created)
                    {
                        foreach (var error in errors)
                        {
                            foreach (var item in error)
                            {
                                Logger.WriteLog("Error occurred while executing HTTP request. Error: " + item);
                            }
                        }
                    }
                }
                else if (retObj.ContainsKey("Message"))
                {
                    return new KeyValuePair<HttpStatusCode, JToken>(key, retObj.ToObject<JToken>());
                }

                return new KeyValuePair<HttpStatusCode, JToken>(key, retObj["result"].ToObject<JToken>());
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Error occurred while executing HTTP request. Error: " + ex.Message);
                Logger.WriteLog(ex.StackTrace);

                return default;
            }
        }
    }
}
