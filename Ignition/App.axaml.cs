namespace Ignition
{
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Ignition.Views;

    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new PrimaryWindow();
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                singleView.MainView = new PrimaryWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
