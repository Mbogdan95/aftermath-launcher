using System;
using System.Collections.Generic;
using System.IO;

namespace Ignition.Api
{
    public static class Logger
    {
        // TODO: IMPLEMENT N LOG
        public static void WriteLog(string messaage)
        {
            string path = Settings.Instance.LauncherData.SettingsLocation + "/Launcher.log";

            List<string> logContent = new List<string>()
            {
                $"{DateTime.Now}",
                messaage
            };

            File.AppendAllLines(path, logContent);
        }
    }
}
