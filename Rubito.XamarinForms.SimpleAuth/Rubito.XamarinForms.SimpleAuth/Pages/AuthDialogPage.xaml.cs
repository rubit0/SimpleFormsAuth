using Rubito.SimpleFormsAuth.Behaviours;
using Rubito.SimpleFormsAuth.Tools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Rubito.SimpleFormsAuth.Pages
{
    public partial class AuthDialogPage : ContentPage
    {
        AuthenticationBehaviour _authenticationBehaviour;
        OAuth2PasswordCredentialsAuthenticator _authenticator;
        Dictionary<FormAuthenticatorField, Entry> _fieldToEntryPairs = new Dictionary<FormAuthenticatorField, Entry>();
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        List<IEntryValidator> _entryValidators = new List<IEntryValidator>();
        AuthPageConfiguration _authPageConfiguration;

        public AuthDialogPage(OAuth2PasswordCredentialsAuthenticator authenticator, AuthPageConfiguration authPageConfiguration = null)
        {
            if (authenticator == null)
                throw new ArgumentNullException(nameof(authenticator), "You must provide an anthenticator");

            if (authPageConfiguration == null)
                authPageConfiguration = new AuthPageConfiguration();

            _authPageConfiguration = authPageConfiguration;
            _authenticator = authenticator;
            _authenticator.Completed += AuthenticatorOnCompleted;
            _authenticator.Error += AuthenticatorOnError;

            InitializeComponent();
            AuthFieldsToEntries();
            ConfigurePage();

            _authenticationBehaviour = ControllerBag.GetBehaviour<AuthenticationBehaviour>();
        }

        protected void ConfigurePage()
        {
            HeaderTitle.Text = _authPageConfiguration.Title;
            HeaderMessage.Text = _authPageConfiguration.SubTitle;
            CloseIcon.IsVisible = _authPageConfiguration.ShowCloseButton;
            RegistrationButton.IsVisible = _authPageConfiguration.ShowRegistrationButton;
        }

        protected override void OnDisappearing()
        {
            _cancellationTokenSource.Cancel();
            _authenticator.Completed -= AuthenticatorOnCompleted;
            _authenticator.Error -= AuthenticatorOnError;

            base.OnDisappearing();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            await _authenticationBehaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Start);

            foreach (var pair in _fieldToEntryPairs)
                pair.Key.Value = pair.Value.Text;

            if (!await ValidateEntries())
                return;

            try
            {
                var account = await _authenticator.SignInAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {

                await _authenticationBehaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Fail, ex.Message);
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            _authenticator.OnError("User closed page.");
            await Navigation.PopModalAsync();
        }

        private async Task<bool> ValidateEntries()
        {
            foreach (var entryValidator in _entryValidators)
            {
                var validation = entryValidator.Validate();

                if (!validation.Item1)
                {
                    await _authenticationBehaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Fail, validation.Item2);
                    return false;
                }
            }

            return true;
        }

        private async void AuthenticatorOnCompleted(object sender, AuthenticatorCompletedEventArgs eventArgs)
        {
            if (eventArgs.IsAuthenticated)
            {
                eventArgs.Account.Properties.Add("RememberMe", InputRememberMe.IsToggled.ToString());
                await _authenticationBehaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Success);
                await Task.Delay(1500);

                await Navigation.PopModalAsync();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            OnCancelClicked(this, EventArgs.Empty);
            return base.OnBackButtonPressed();
        }

        private async void AuthenticatorOnError(object sender, AuthenticatorErrorEventArgs eventArgs)
        {
            await _authenticationBehaviour.SwitchAuthState(AuthenticationBehaviour.AuthState.Fail, eventArgs.Message);
        }

        protected virtual void AuthFieldsToEntries()
        {
            var fields = _authenticator.Fields;

            foreach (var field in fields)
            {
                var entry = new Entry
                {
                    Placeholder = field.Placeholder,
                    Text = field.Value,
                };

                var behaviour = new EntrySanityBehaviour { DoRemoveWhiteSpace = true };
                _entryValidators.Add(behaviour);
                entry.Behaviors.Add(behaviour);

                switch (field.FieldType)
                {
                    case FormAuthenticatorFieldType.PlainText:
                        if (field.Key == "username")
                            behaviour.MinCharLength = _authPageConfiguration.MinUsernameLength;

                        entry.Keyboard = Keyboard.Text;
                        break;
                    case FormAuthenticatorFieldType.Email:
                        entry.Keyboard = Keyboard.Email;
                        behaviour.CheckIsEmail = true;
                        break;
                    case FormAuthenticatorFieldType.Password:
                        entry.IsPassword = true;
                        entry.Keyboard = Keyboard.Text;
                        behaviour.MinCharLength = _authPageConfiguration.MinPasswordLength;
                        break;
                    default:
                        entry.Keyboard = Keyboard.Text;
                        break;
                }

                _fieldToEntryPairs.Add(field, entry);

                InputEntryContainer.Children.Add(entry);
            }
        }
    }
}
