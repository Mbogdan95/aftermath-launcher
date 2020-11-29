﻿namespace Ignition.ViewModels
{
    using Avalonia.Media;
    using Ignition.Api;
    using Ignition.Models;
    using Newtonsoft.Json.Linq;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
    using System.Net;
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

            KeyValuePair<HttpStatusCode, JObject> result = await Utils.Utils.PutRequest("/api/auth/login", dynamicObject);

            if (result.Key is HttpStatusCode.OK)
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
                    Avatar = Utils.Utils.GetImageFromUrl(result.Value["Avatar"].ToString()),
                    //AccSig = result.Value["FlhookUser"]["loginSignature"].ToObject<uint>(),
                    //AccCode = result.Value["FlhookUser"]["loginCode"].ToObject<uint>(),
                    PlayerID = result.Value["FlhookUser"]["id"].ToString()
                };

                HideLoginWindow();
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
                    var loggedIn = await Utils.Utils.GetRequest("/api/auth/");
                    if (loggedIn.Key == HttpStatusCode.OK)
                    {
                        primaryWindowViewModel.LoggedUser = new User()
                        {
                            Username = loggedIn.Value["user"]["Username"].ToString(),
                            Credits = loggedIn.Value["user"]["FlhookUser"]["bankCash"].ToObject<long>().ToString("#,##0,,", CultureInfo.InvariantCulture),
                            WarningLevel = "110%",
                            Level = 9000,
                            Rank = "SNAC Lover",
                            Avatar = Utils.Utils.GetImageFromUrl(loggedIn.Value["user"]["Avatar"].ToString()),
                            //AccSig = loggedIn.Value["user"]["FlhookUser"]["loginSignature"].ToObject<uint>(),
                            //AccCode = loggedIn.Value["user"]["FlhookUser"]["loginCode"].ToObject<uint>(),
                            PlayerID = loggedIn.Value["user"]["FlhookUser"]["id"].ToString()
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
