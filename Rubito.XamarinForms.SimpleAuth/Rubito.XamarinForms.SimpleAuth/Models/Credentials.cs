using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Rubito.XamarinForms.SimpleAuth.Models
{
    public class Credentials
    {
        public readonly string Username;
        public readonly string Password;
        public readonly string Scope;
        public bool RememberMe { get; set; }
        public bool StorePassword { get; set; }

        public Credentials(string username, string password, string scope = null)
        {
            Username = username;
            Password = password;
            Scope = scope;
        }

        public FormUrlEncodedContent ToFormUrlEncodedContent()
        {
            var credentialsEncoded = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", this.Username},
                {"password", this.Password}
            };

            if (!string.IsNullOrEmpty(this.Scope))
                credentialsEncoded.Add("scope", this.Scope);

            return new FormUrlEncodedContent(credentialsEncoded);
        }

        public CredentialsValidationResult Validate(uint minUsernameLength = 4, uint minPasswordLength = 6, bool isEmail = false)
        {
            List<string> messages = new List<string>();
            var isValid = true;

            if (string.IsNullOrEmpty(Username))
            {
                messages.Add("Enter your Username.");
                isValid = false;
            }
            else
            {
                if (isEmail && !IsValidEmail(Username))
                {
                    messages.Add("Invalid email address.");
                    isValid = false;
                }

                if (Username.Length < minUsernameLength)
                {
                    messages.Add($"Username must be {minUsernameLength} characters long.");
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(Password))
            {
                messages.Add("Enter a Password.");
                isValid = false;
            }
            else
            {
                if (Password.Length < minPasswordLength)
                {
                    messages.Add($"Password must be {minPasswordLength} characters long.");
                    isValid = false;
                }
            }

            return new CredentialsValidationResult(isValid, messages);
        }

        private bool IsValidEmail(string input)
        {
            var result = Regex.IsMatch(input, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return result;
        }
    }
}
