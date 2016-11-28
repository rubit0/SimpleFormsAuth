using Rubito.XamarinForms.SimpleAuth.Behaviours;
using Rubito.XamarinForms.SimpleAuth.Models;
using Rubito.XamarinForms.SimpleAuth.Tools;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rubito.XamarinForms.SimpleAuth.Pages
{
    public partial class AuthDialogPage : ContentPage
    {
        private readonly Uri _tokenEndpoint;
        private readonly HttpMessageHandler _httpMessageHandler;
        private readonly AuthenticationBehaviour _behaviour;
        private Action<AuthenticationResult> _resultcallback;

        public AuthDialogPage(Uri tokenEndpoint, Action<AuthenticationResult> resultCallback, HttpMessageHandler httpMessageHandler = null)
        {
            if (tokenEndpoint == null || resultCallback == null)
                throw new ArgumentNullException();

            _tokenEndpoint = tokenEndpoint;
            _resultcallback += resultCallback;
            _httpMessageHandler = httpMessageHandler;

            InitializeComponent();

            Device.OnPlatform(iOS: () => HeroIcon.TranslationY -= 24);

            LoadUser();
            _behaviour = ControllerBag.GetBehaviour<AuthenticationBehaviour>();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            _resultcallback.Invoke(new AuthenticationResult(false, "User closed dialog."));
            await Navigation.PopModalAsync();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            var credentials = new Credentials(InputEmail.Text, InputPassword.Text);
            var credentialValidation = credentials.Validate();
            if (!credentialValidation.IsValid)
            {
                await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Fail,
                    credentialValidation.GetFormatedErrors());

                return;
            }

            await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Start);

            var authenticator = new ClientCredentialsAuthenticator(_tokenEndpoint, credentials);
            var result = await authenticator.AuthenticateAsync(_httpMessageHandler);
            if (result.IsAuthenticated)
            {
                await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Success);
                await Task.Delay(1500);

                if (InputRememberMe.IsToggled)
                    AuthPersistence.StoreUsername(result.Token.UserName);

                _resultcallback.Invoke(result);

                await Navigation.PopModalAsync();
            }
            else
            {
                await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Fail, result.Message);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            OnCancelClicked(this, EventArgs.Empty);

            return base.OnBackButtonPressed();
        }

        private void LoadUser()
        {
            var username = AuthPersistence.LoadUsername();
            if (!string.IsNullOrEmpty(username))
            {
                InputEmail.Text = username;
                InputRememberMe.IsToggled = true;
            }

            var password = AuthPersistence.LoadPassword();
            if (!string.IsNullOrEmpty(password))
            {
                InputPassword.Text = password;
                InputSavePassword.IsToggled = true;
            }
        }

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
