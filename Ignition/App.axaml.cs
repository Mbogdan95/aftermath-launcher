using System;
using System.Threading;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Threading;

namespace Ignition
{
    using Avalonia;
    using Avalonia.Controls;
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

            base.OnFrameworkInitializationCompleted();
        }

        internal static void TriggerShake(Control control)
        {
            
        }
    }
}
