using System;
using System.Collections.Generic;
using System.IO;

namespace Ignition.Api
{
    public static class Logger
    {
        public static void WriteLog(string messaage)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Aftermath/Launcher/Launcher.log";

            List<string> logContent = new List<string>()
            {
                $"{DateTime.Now.ToString()}",
                messaage
            };

            File.AppendAllLines(path, logContent);
        }
    }
}
