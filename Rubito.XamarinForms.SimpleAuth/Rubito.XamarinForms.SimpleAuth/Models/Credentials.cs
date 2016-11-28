using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Xamarin.Auth;

namespace Rubito.XamarinForms.SimpleAuth.Models
{
    public class Credentials
    {
        FormAuthenticatorField _username = new FormAuthenticatorField
        {
            FieldType = FormAuthenticatorFieldType.PlainText,
            Key = "username",
            Title = "Username",
            Placeholder = "Username"
        };

        FormAuthenticatorField _password = new FormAuthenticatorField
        {
            FieldType = FormAuthenticatorFieldType.Password,
            Key = "password",
            Title = "Password",
            Placeholder = "Password"
        };

        public FormAuthenticatorField Username
        {
            get { return this._username; }
        }

        public FormAuthenticatorField Password
        {
            get { return this._password; }
        }

        public bool RememberMe { get; set; }
        public bool StorePassword { get; set; }

        public Credentials(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username), "you must provide a username.");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "you must provide a username.");

            this._username.Value = username;
            this._password.Value = password;
        }

        public IDictionary<string, string> ToQuerryValues()
        {
            var credentialsEncoded = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", this.Username.Value},
                {"password", this.Password.Value}
            };

            return credentialsEncoded;
        }

        public FormUrlEncodedContent ToFormUrlEncodedContent()
        {
            return new FormUrlEncodedContent(ToQuerryValues());
        }

        public CredentialsValidationResult Validate(uint minUsernameLength = 4, uint minPasswordLength = 6, bool isEmail = false)
        {
            List<string> messages = new List<string>();
            var isValid = true;

            if (string.IsNullOrEmpty(Username.Value))
            {
                messages.Add("Enter your Username.");
                isValid = false;
            }
            else
            {
                if (isEmail && !IsValidEmail(Username.Value))
                {
                    messages.Add("Invalid email address.");
                    isValid = false;
                }

                if (Username.Value.Length < minUsernameLength)
                {
                    messages.Add($"Username must be {minUsernameLength} characters long.");
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(Password.Value))
            {
                messages.Add("Enter a Password.");
                isValid = false;
            }
            else
            {
                if (Password.Value.Length < minPasswordLength)
                {
                    messages.Add($"Password must be {minPasswordLength} characters long.");
                    isValid = false;
                }
            }

            return new CredentialsValidationResult(isValid, messages);
        }

        private bool IsValidEmail(string input)
        {
            //http://stackoverflow.com/a/16168103
            var result = Regex.IsMatch(input, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return result;
        }
    }
}
