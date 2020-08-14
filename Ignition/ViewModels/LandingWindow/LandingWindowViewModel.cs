namespace Ignition.ViewModels.LandingWindow
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public class LandingWindowViewModel : ViewModelBase
    {
        public string IgVersion => Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion).ToString();
        public string GameVersion { get; private set; }

        public LandingWindowViewModel()
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(@"D:\Games\Aftermath\EXE\Freelancer.exe");
            string[] arr = info.FileVersion.Split(", ");
            GameVersion = $"{arr[0]}.{arr[1]}.{arr[2]}";
        }
    }
}