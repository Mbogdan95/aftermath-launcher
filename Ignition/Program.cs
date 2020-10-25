namespace Ignition
{
    using Avalonia;
    using Avalonia.Logging.Serilog;
    using Avalonia.ReactiveUI;
    using Projektanker.Icons.Avalonia;
    using Projektanker.Icons.Avalonia.FontAwesome;

    public class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
#if _WINDOWS
                .UseDirect2D1()
                .UseWin32()
#else
                .UsePlatformDetect()
#endif
                .AfterSetup(AfterSetupCallback)
                .LogToDebug()
                .UseReactiveUI();

        // Called after setup
        private static void AfterSetupCallback(AppBuilder appBuilder)
        {
            // Register icon provider(s)
            IconProvider.Register<FontAwesomeIconProvider>();
        }
    }
}
