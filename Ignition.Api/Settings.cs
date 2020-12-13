using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace Ignition.Api
{
    public class Settings
    {
        public static Settings Instance { get; private set; } = new Settings();

        public Data LauncherData { get; private set; }

        private static string settingsLocation;
        private static string fileLocation;

        private Settings()
        {
            fileLocation = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            settingsLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Aftermath/Launcher";
            Directory.CreateDirectory(settingsLocation);

            LoadData();
        }

        private void SaveData()
        {
            var content = JsonConvert.SerializeObject(LauncherData);

            File.WriteAllText(settingsLocation + "/config.json", content);
        }

        private void LoadData()
        {
            if (File.Exists(settingsLocation + "/config.json"))
            {
                try
                {
                    string content = File.ReadAllText(settingsLocation + "/config.json");

                    LauncherData = JsonConvert.DeserializeObject<Data>(content);
                }
                catch (Exception ex)
                {
                    GenerateDefaultConfig();

                    Logger.WriteLog("Unable to read config.json. Error: " + ex.Message);
                }
            }
            else
            {
                GenerateDefaultConfig();
            }
        }

        public void GenerateDefaultConfig()
        {
            DirectoryInfo d = new DirectoryInfo(Process.GetCurrentProcess().MainModule.FileName);

            if (d.Parent == null)
            {
                throw new IOException("Launcher process was at drive root which is forbidden.");
            }

            LauncherData = new Data()
            {
                AftermathInstall = fileLocation + "/../Game",
                SettingsLocation = settingsLocation,
                PatchServer = "https://files.aftermath.space/Launcher/GameData",
                NewsLocation = "https://files.aftermath.space/Launcher/news.json",
                ApiLocation = "https://api.aftermath.space",
                DefaultDesktopResolution = true,
                WindowedMode = false
            };

            Directory.CreateDirectory(LauncherData.AftermathInstall);

            SaveData();
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

        public void SetResolution(int w, int h)
        {
            LauncherData.WidthResolution = w;
            LauncherData.HeightResolution = h;

            SaveData();
        }

        public class Data
        {
            public string AftermathInstall { get; set; }

            public string SettingsLocation { get; set; }

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
