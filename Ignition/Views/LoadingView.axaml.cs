using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ignition.ViewModels;
using System;
using System.Threading.Tasks;

namespace Ignition.Views
{
    public class LoadingView : UserControl
    {
        public LoadingView()
        {
            InitializeComponent();

            Initialized += async (s, e) => await LoadingViewInitialized(s, e);
        }

        private async Task LoadingViewInitialized(object sender, EventArgs e)
        {
            LoadingViewModel loadingViewModel = (LoadingViewModel)DataContext;

            bool loadNewsExecuted = await loadingViewModel.LoadNews();

            if (loadNewsExecuted)
            {
                loadingViewModel.ChangeView();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
