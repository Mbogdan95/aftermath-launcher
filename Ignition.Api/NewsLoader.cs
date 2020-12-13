using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Ignition.Api
{
    public static class NewsLoader
    {
        public class NewsItem
        {
            public string Title { get; set; }

            public string Date { get; set; }

            public string Description { get; set; }

            public string NewsUrl { get; set; }

            public Bitmap Image { get; set; }
        }
        public static async Task<(bool, List<NewsItem>, List<NewsItem>)> LoadNews()
        {
            // Initialize variables
            var siriusNews = new List<NewsItem>();
            var modNews = new List<NewsItem>();

            using WebClient webClient = new WebClient();

            try
            {
                // Get JSON string from link
                string jsonString = webClient.DownloadString(Settings.Instance.LauncherData.NewsLocation);

                var blogs = await WebRequest.GetRequest("/api/blog?page=1&count=4");

                foreach (var item in blogs.Value)
                {
                    try
                    {
                        modNews.Add(ReadBlog(item));
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("Unknown exception - LoadNews. ERROR: " + ex.Message);
                        Logger.WriteLog(ex.StackTrace);
                    }
                }

                // Loop through each JObject
                foreach (JObject item in JsonConvert.DeserializeObject<JArray>(jsonString).ToObject<List<JObject>>())
                {
                    try
                    {
                        siriusNews.Add(ReadNews(item));
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("Unknown exception - LoadNews. ERROR: " + ex.Message);
                        Logger.WriteLog(ex.StackTrace);
                    }
                }
            }
            catch (WebException webEx)
            {
                Logger.WriteLog("Unable to download resources");
                Logger.WriteLog(webEx.StackTrace);

                return (false, null, null);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Unknown exception - LoadNews. ERROR: " + ex.Message);
                Logger.WriteLog(ex.StackTrace);

                return (false, null, null);
            }

            return (true, siriusNews, modNews);
        }

        private static NewsItem ReadBlog(JToken item)
        {
            byte[] bytes = Convert.FromBase64String(item["Banner"]["data"].ToString());

            Bitmap image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = new Bitmap(ms);
            }

            return new NewsItem()
            {
                Title = item["Title"].ToString(),
                Description = item["Description"].ToString(),
                Date = DateTime.Parse(item["CreationDate"].ToString()).ToString(),
                Image = image,
                NewsUrl = $"https://forums.aftermath.space/blogs/{item["Id"]}"
            };
        }

        private static NewsItem ReadNews(JObject item)
        {
            // Create news item
            return new NewsItem()
            {
                Title = item["title"].ToString(),
                Description = "  " + item["subtitle"].ToString(),
                Date = item["date"].ToString(),
                Image = WebRequest.GetImageFromUrl(item["imageUrl"].ToString()),
                NewsUrl = item["url"].ToString()
            };
        }
    }
}
