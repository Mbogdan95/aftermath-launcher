using Newtonsoft.Json;
using System;
using System.IO;

namespace Ignition.Api
{
    public class Settings
    {
        public static Settings Instance { get; private set; } = new Settings();

        public Data LauncherData { get; private set; }

        private static string fileLocation;

        private Settings()
        {
            fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Aftermath/Launcher";
            Directory.CreateDirectory(fileLocation);

            LoadData();
        }

        private void SaveData()
        {
            var content = JsonConvert.SerializeObject(LauncherData);

            File.WriteAllText(fileLocation + "/config.json", content);
        }

        private void LoadData()
        {
            bool valid = File.Exists(fileLocation + "/config.json");

            if (valid)
            {
                try
                {
                    string content = File.ReadAllText(fileLocation + "/config.json");

                    LauncherData = JsonConvert.DeserializeObject<Data>(content);
                }
                catch (Exception)
                {
                    // TODO: LOG ERROR

                    valid = false;
                }
            }

            if (!valid)
            {
                this.LauncherData = new Data()
                {
                    AftermathInstall = fileLocation + "/../Game",
                    PatchServer = "https://patch.aftermath.space",
                    NewsLocation = "https://files.aftermath.space/Launcher/news.json",
                    ApiLocation = "https://api.aftermath.space",
                    DefaultDesktopResolution = true,
                    WindowedMode = false
                };
            }
        }

        public void SetToken(string token)
        {
            LauncherData.Token = token;

            SaveData();
        }

        public void SetWindowedMode(bool value)
        {
            LauncherData.WindowedMode = value;

            SaveData();
        }

        public void SetDefaultDesktopResolution(bool value)
        {
            LauncherData.DefaultDesktopResolution = value;

            SaveData();
        }

        public void SetStayLoggedIn(bool value)
        {
            LauncherData.StayLoggedIn = value;

            SaveData();
        }

        public void SetWidthResolution(int value)
        {
            LauncherData.WidthResolution = value;

            SaveData();
        }

        public void SetHeightResolution(int value)
        {
            LauncherData.HeightResolution = value;

            SaveData();
        }

        public class Data
        {
            public string AftermathInstall { get; set; }

            public string PatchServer { get; set; }

            public string NewsLocation { get; set; }

            public string Token { get; set; }

            public string ApiLocation { get; set; }

            public bool DefaultDesktopResolution { get; set; }

            public bool WindowedMode { get; set; }

            public bool StayLoggedIn { get; set; }

            public int WidthResolution { get; set; }

            public int HeightResolution { get; set; }
        }
    }
}
