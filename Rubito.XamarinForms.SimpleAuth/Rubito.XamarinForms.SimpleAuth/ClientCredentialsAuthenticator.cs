using ModernHttpClient;
using Rubito.XamarinForms.SimpleAuth.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rubito.XamarinForms.SimpleAuth.Pages;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Utilities;

namespace Rubito.XamarinForms.SimpleAuth
{
    public class ClientCredentialsAuthenticator : FormAuthenticator
    {
        bool _usernameIsEmail;
        string _scope;
        Uri _accessTokenUrl;

        Uri AccessTokenUrl
        {
            get { return this._accessTokenUrl; }
        }

        public ClientCredentialsAuthenticator(Uri accessTokenUrl, Uri createAccountLink = null, string scope = null) : base(createAccountLink)
        {
            if (accessTokenUrl == null)
                throw new ArgumentNullException(nameof(accessTokenUrl), "an accessTokenUrl must be provided");

            this._scope = scope;
            this.InitializeFormFields();
            this._accessTokenUrl = accessTokenUrl;
        }

        public override async Task<Account> SignInAsync(CancellationToken cancellationToken)
        {
            var result = await RequestAccessTokenAsync();

            if (result == null)
                OnError("Server response contained invalid data.");

            if (result.ContainsKey("error"))
                OnError(result["error"]);

            if (result.ContainsKey("access_token"))
            {
                var account = new Account(result["user"], result);
                OnSucceeded(account);
                return account;
            }
            
            return null;
        }

        public virtual ContentPage GetFormsUI()
        {
            return new AuthDialogPage(this);
        }

        protected async Task<IDictionary<string, string>> RequestAccessTokenAsync()
        {
            var query = FieldsToFormUrlEncodedContent();
            var client = new HttpClient(new NativeMessageHandler());

            using (client)
            {
                var response = await client.PostAsync(_accessTokenUrl, query);
                var contentStream = await response.Content.ReadAsStringAsync();
                var data = contentStream.Contains("{")
                    ? WebEx.JsonDecode(contentStream)
                    : WebEx.FormDecode(contentStream);

                if (data.ContainsKey("access_token") || data.ContainsKey("error"))
                    return data;

                throw new AuthException("Expected an access_token or error message in the response.");
            }
        }

        protected void InitializeFormFields()
        {
            var username = new FormAuthenticatorField
            {
                FieldType = (_usernameIsEmail)
                    ? FormAuthenticatorFieldType.Email
                    : FormAuthenticatorFieldType.PlainText,
                Key = "username",
                Title = "Username",
                Placeholder = "Username"
            };

            var password = new FormAuthenticatorField
            {
                FieldType = FormAuthenticatorFieldType.Password,
                Key = "password",
                Title = "Password",
                Placeholder = "Password"
            };

            Fields.Add(username);
            Fields.Add(password);
        }

        protected virtual FormUrlEncodedContent FieldsToFormUrlEncodedContent()
        {
            var credentialsEncoded = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", GetFieldValue("username")},
                {"password", GetFieldValue("password")}
            };

            return new FormUrlEncodedContent(credentialsEncoded);
        }
    }
}
