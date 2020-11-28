using System;
using System.Diagnostics;

namespace Ignition
{
    using Avalonia;
    using Avalonia.ReactiveUI;
    using Projektanker.Icons.Avalonia;
    using Projektanker.Icons.Avalonia.FontAwesome;

    public class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
                // MessageBoxManager.GetMessageBoxStandardWindow("An exception has occured.", "An error has occured within the program and it must terminate.\n\nAdditional Information: " + ex.Message).Show();
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .AfterSetup(AfterSetupCallback)
                .UseReactiveUI();

        // Called after setup
        private static void AfterSetupCallback(AppBuilder appBuilder)
        {
            // Register icon provider(s)
            IconProvider.Register<FontAwesomeIconProvider>();
        }
    }
}
