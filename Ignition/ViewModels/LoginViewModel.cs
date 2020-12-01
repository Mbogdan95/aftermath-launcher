using System.Net;

namespace Ignition.ViewModels
{
    using Avalonia.Media;
    using Ignition.Api;
    using Ignition.Models;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
    using System.Net.Mail;
    using System.Reactive;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public class LoginViewModel : BaseViewModel
    {
        public ReactiveCommand<Unit, Task> Login { get; }
        public ReactiveCommand<Unit, Unit> ForgottenPassword { get; }
        public ReactiveCommand<Unit, Unit> NeedAnAccount { get; }

        [Reactive]
        public string Email { get; set; }

        [Reactive]
        public string Password { get; set; }

        public string LoginError { get; set; }

        public SolidColorBrush LoginErrorColor { get; set; }

        private PrimaryWindowViewModel primaryWindowViewModel;

        public LoginViewModel(PrimaryWindowViewModel primaryWindowViewModel)
        {
            this.primaryWindowViewModel = primaryWindowViewModel;

            Login = ReactiveCommand.Create(LoginButtonClick);
            ForgottenPassword = ReactiveCommand.Create(ForgottenPasswordButtonClick);
            NeedAnAccount = ReactiveCommand.Create(NeedAnAccountButtonClick);

            AutoLogIn();
        }

        private async Task LoginButtonClick()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                LoginError = "Email field is empty";
                LoginErrorColor = new SolidColorBrush(Colors.Red);

                this.RaisePropertyChanged(nameof(LoginErrorColor));
                this.RaisePropertyChanged(nameof(LoginError));

                return;
            }
            else
            {
                try
                {
                    MailAddress m = new MailAddress(Email);
                }
                catch (FormatException)
                {
                    LoginError = "Email is invalid";
                    LoginErrorColor = new SolidColorBrush(Colors.Red);

                    this.RaisePropertyChanged(nameof(LoginErrorColor));
                    this.RaisePropertyChanged(nameof(LoginError));

                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                LoginError = "Password field is empty";
                LoginErrorColor = new SolidColorBrush(Colors.Red);

                this.RaisePropertyChanged(nameof(LoginErrorColor));
                this.RaisePropertyChanged(nameof(LoginError));

                return;
            }

            dynamic dynamicObject = new ExpandoObject();
            dynamicObject.Email = Email;
            dynamicObject.Password = Password;

            var result = await WebRequest.PutRequest("/api/auth/login", dynamicObject);

            if (result.Key == HttpStatusCode.OK)
            {
                Debug.WriteLine("Login successful");

                Settings.Instance.SetToken(result.Value["Token"].ToString());

                primaryWindowViewModel.LoggedUser = new User()
                {
                    Username = result.Value["Username"].ToString(),
                    Credits = result.Value["FlhookUser"]["bankCash"].ToObject<long>().ToString("#,##0,,", CultureInfo.InvariantCulture),
                    WarningLevel = "110%",
                    Level = 9000,
                    Rank = "SNAC Lover",
                    Avatar = WebRequest.GetImageFromUrl(result.Value["Avatar"].ToString()),
                    AccSig = result.Value["FlhookUser"]["loginSignature"]?.ToString(),
                    AccCode = result.Value["FlhookUser"]["loginCode"]?.ToString(),
                    PlayerID = result.Value["FlhookUser"]["id"].ToObject<long>()
                };

                HideLoginWindow();
            }
            else if (result.Key == 0)
            {
                LoginError = "Connection error";
                LoginErrorColor = new SolidColorBrush(Colors.Red);

                this.RaisePropertyChanged(nameof(LoginErrorColor));
                this.RaisePropertyChanged(nameof(LoginError));
            }
            else
            {
                LoginError = "Email and password do not match";
                LoginErrorColor = new SolidColorBrush(Colors.Red);

                this.RaisePropertyChanged(nameof(LoginErrorColor));
                this.RaisePropertyChanged(nameof(LoginError));
            }
        }

        private void AutoLogIn()
        {
            if (Settings.Instance.LauncherData.StayLoggedIn)
            {
                Task.Run(new Action(async () =>
                {
                    var loggedIn = await WebRequest.GetRequest("/api/auth/");
                    if (loggedIn.Key == HttpStatusCode.OK)
                    {
                        primaryWindowViewModel.LoggedUser = new User()
                        {
                            Username = loggedIn.Value["user"]["Username"].ToString(),
                            Credits = loggedIn.Value["user"]["FlhookUser"]["bankCash"].ToObject<long>().ToString("#,##0,,", CultureInfo.InvariantCulture),
                            WarningLevel = "110%",
                            Level = 9000,
                            Rank = "SNAC Lover",
                            Avatar = WebRequest.GetImageFromUrl(loggedIn.Value["user"]["Avatar"].ToString()),
                            AccSig = loggedIn.Value["user"]["FlhookUser"]["loginSignature"]?.ToString(),
                            AccCode = loggedIn.Value["user"]["FlhookUser"]["loginCode"]?.ToString(),
                            PlayerID = loggedIn.Value["user"]["FlhookUser"]["id"].ToObject<long>()
                        };

                        HideLoginWindow();
                    }
                }));
            }
        }

        private void HideLoginWindow()
        {
            primaryWindowViewModel.OnUpdateView("LandingPage");
        }

        private void NeedAnAccountButtonClick()
        {
            string url = "https://forums.aftermath.space/signup";

            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void ForgottenPasswordButtonClick()
        {
            string url = "https://forums.aftermath.space/login";

            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
