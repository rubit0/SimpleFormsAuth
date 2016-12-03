using Rubito.XamarinForms.SimpleAuth.Behaviours;
using Rubito.XamarinForms.SimpleAuth.Tools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Rubito.XamarinForms.SimpleAuth.Pages
{
    public partial class AuthDialogPage : ContentPage
    {
        AuthenticationBehaviour _behaviour;
        OAuth2PasswordCredentialsAuthenticator _authenticator;
        Dictionary<FormAuthenticatorField, Entry> _fieldToEntryPairs;
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public AuthDialogPage(OAuth2PasswordCredentialsAuthenticator authenticator)
        {
            if (authenticator == null)
                throw new ArgumentNullException(nameof(authenticator), "You must provide an anthenticator");

            _authenticator = authenticator;
            _authenticator.Completed += AuthenticatorOnCompleted;
            _authenticator.Error += AuthenticatorOnError;

            InitializeComponent();
            AuthFieldsToEntries();

            //TODO Add Register button

            Device.OnPlatform(iOS: () => HeroIcon.TranslationY -= 24);
            _behaviour = ControllerBag.GetBehaviour<AuthenticationBehaviour>();
        }

        protected override void OnDisappearing()
        {
            _cancellationTokenSource.Cancel();
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

                await Navigation.PopModalAsync();
            }
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Start);

            //TODO Implement input validation

            foreach (var pair in _fieldToEntryPairs)
                pair.Key.Value = pair.Value.Text;

            try
            {
                var account = await _authenticator.SignInAsync(new CancellationToken());
            }
            catch (Exception ex)
            {

                await _behaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Fail, ex.Message);
            }
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

        protected virtual void AuthFieldsToEntries()
        {
            if (_fieldToEntryPairs == null)
                _fieldToEntryPairs = new Dictionary<FormAuthenticatorField, Entry>();

            var fields = _authenticator.Fields;

            foreach (var field in fields)
            {
                var entry = new Entry
                {
                    Placeholder = field.Placeholder,
                    Text = field.Value,
                    Behaviors = { new InputFormatterBehaviour() }
                };

                switch (field.FieldType)
                {
                    case FormAuthenticatorFieldType.PlainText:
                        entry.Keyboard = Keyboard.Text;
                        break;
                    case FormAuthenticatorFieldType.Email:
                        entry.Keyboard = Keyboard.Email;
                        break;
                    case FormAuthenticatorFieldType.Password:
                        entry.IsPassword = true;
                        entry.Keyboard = Keyboard.Text;
                        break;
                    default:
                        entry.Keyboard = Keyboard.Text;
                        throw new ArgumentOutOfRangeException();
                }

                _fieldToEntryPairs.Add(field, entry);

                InputEntryContainer.Children.Add(entry);
            }
        }

        private async void OnStorePasswordToggled(object sender, ToggledEventArgs e)
        {
            var swith = sender as Switch;
            if (swith != null && swith.IsToggled)
                await DisplayAlert("Security warning", "It is not advised to save the password on your device.", "Ok");
        }

        private void OnRememberMeToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
                return;
        }
    }
}
