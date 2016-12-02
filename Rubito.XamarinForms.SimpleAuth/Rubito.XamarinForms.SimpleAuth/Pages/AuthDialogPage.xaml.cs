using Rubito.XamarinForms.SimpleAuth.Behaviours;
using Rubito.XamarinForms.SimpleAuth.Tools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Rubito.XamarinForms.SimpleAuth.Pages
{
    public partial class AuthDialogPage : ContentPage
    {
        //private readonly Uri _tokenEndpoint;
        //private readonly HttpMessageHandler _httpMessageHandler;
        private readonly AuthenticationBehaviour _behaviour;
        //private Action<AuthenticationResult> _resultcallback;
        private OAuth2PasswordCredentialsAuthenticator _authenticator;

        public AuthDialogPage(OAuth2PasswordCredentialsAuthenticator authenticator)
        {
            if (authenticator == null)
                throw new ArgumentNullException(nameof(authenticator), "You must provide an anthenticator");

            _authenticator = authenticator;
            _authenticator.Completed += AuthenticatorOnCompleted;
            _authenticator.Error += AuthenticatorOnError;

            InitializeComponent();

            //TODO Loading User from Store
            //TODO Use fields to build entries

            Device.OnPlatform(iOS: () => HeroIcon.TranslationY -= 24);
            _behaviour = ControllerBag.GetBehaviour<AuthenticationBehaviour>();
        }

        protected override void OnDisappearing()
        {
            _authenticator.Completed -= AuthenticatorOnCompleted;
            _authenticator.Error -= AuthenticatorOnError;

            base.OnDisappearing();
        }

        private async void AuthenticatorOnError(object sender, AuthenticatorErrorEventArgs eventArgs)
        {
            await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Fail, eventArgs.Message);
        }

        private async void AuthenticatorOnCompleted(object sender, AuthenticatorCompletedEventArgs eventArgs)
        {
            if (eventArgs.IsAuthenticated)
            {
                await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Success);
                await Task.Delay(1500);

                if (InputRememberMe.IsToggled)
                    //TODO Implement saving

                    //TODO Implament event that modal has finished displaying

                    await Navigation.PopModalAsync();
            }
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Start);

            //TODO Implement input validation

            _authenticator.Fields.Single(f => f.Value == "username").Value = InputEmail.Text;
            _authenticator.Fields.Single(f => f.Value == "password").Value = InputPassword.Text;

            await _authenticator.SignInAsync(new CancellationToken());
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            _authenticator.OnError("User closed page.");
            await Navigation.PopModalAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            OnCancelClicked(this, EventArgs.Empty);
            return base.OnBackButtonPressed();
        }

        //private void LoadUser()
        //{
        //    var username = AuthPersistence.LoadUsername();
        //    if (!string.IsNullOrEmpty(username))
        //    {
        //        InputEmail.Text = username;
        //        InputRememberMe.IsToggled = true;
        //    }

        //    var password = AuthPersistence.LoadPassword();
        //    if (!string.IsNullOrEmpty(password))
        //    {
        //        InputPassword.Text = password;
        //        InputSavePassword.IsToggled = true;
        //    }
        //}

        private async void OnStorePasswordToggled(object sender, ToggledEventArgs e)
        {
            var swith = sender as Switch;
            if (swith != null && swith.IsToggled)
            {
                await DisplayAlert("Security warning", "It is not advised to save the password on your device.", "Ok");
                AuthPersistence.StorePassword(InputPassword.Text);
            }
            else
            {
                AuthPersistence.DeletePassword();
            }
        }

        private void OnRememberMeToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
                return;

            AuthPersistence.DeleteUsername();
        }
    }
}
