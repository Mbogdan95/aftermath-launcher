using System.Reactive;
using Avalonia.Interactivity;
using ReactiveUI;

namespace Ignition.Views
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class PrimaryWindow : Window
    {
        public PrimaryWindow()
        {
            this.InitializeComponent();
            #if DEBUG
            this.AttachDevTools();
            #endif
            this.HasSystemDecorations = false;
            this.CanResize = false;

            Change2 = ReactiveCommand.Create(Change);
        }

        public ReactiveCommand<Unit, Unit> Change2 { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Change()
        {
            this.FindControl<LandingWindow.LandingWindow>("LandingWindow").Opacity = 1;
        }
    }
}
