using ModernHttpClient;
using Rubito.XamarinForms.SimpleAuth.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Utilities;

namespace Rubito.XamarinForms.SimpleAuth
{
    public class ClientCredentialsAuthenticator : FormAuthenticator
    {
        bool _usernameIsEmail;
        Credentials _credentials;
        Uri _accessTokenUrl;

        Uri AccessTokenUrl
        {
            get { return this._accessTokenUrl; }
        }

        public ClientCredentialsAuthenticator(Uri accessTokenUrl, Credentials credentials) : base(accessTokenUrl)
        {
            if (accessTokenUrl == null)
                throw new ArgumentNullException(nameof(accessTokenUrl), "an accessTokenUrl must be provided");

            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials), "credentials must be provided");

            this.InitializeFormFields();
            this._accessTokenUrl = accessTokenUrl;
            this._credentials = credentials;
        }

        public ClientCredentialsAuthenticator(Uri accessTokenUrl, string username, string password)
        {
            if (accessTokenUrl == null)
                throw new ArgumentNullException(nameof(accessTokenUrl), "an accessTokenUrl must be provided");

            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username), "an username must be provided");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "an password must be provided");

            this.InitializeFormFields();
            this._accessTokenUrl = accessTokenUrl;
            this._credentials = new Credentials(username, password);
        }

        public override async Task<Account> SignInAsync(CancellationToken cancellationToken)
        {
            var result = await AuthenticateAsync();
            return new Account(result["user"], result);
        }

        protected async Task<IDictionary<string, string>> AuthenticateAsync()
        {
            var query = _credentials.ToFormUrlEncodedContent();
            var client = new HttpClient(new NativeMessageHandler());

            using (client)
            {
                var response = await client.PostAsync(_accessTokenUrl, query);
                var contentStream = await response.Content.ReadAsStringAsync();
                var data = contentStream.Contains("{")
                    ? WebEx.JsonDecode(contentStream)
                    : WebEx.FormDecode(contentStream);

                if (data.ContainsKey("error"))
                    throw new AuthException($"Error authenticating: {data["error"]}");
                else if (data.ContainsKey("access_token"))
                    return data;
                else
                    throw new AuthException("Expected an access_token in the response.");
            }
        }

        protected void InitializeFormFields()
        {
            FormAuthenticatorField username = new FormAuthenticatorField
            {
                FieldType = (_usernameIsEmail)
                    ? FormAuthenticatorFieldType.Email
                    : FormAuthenticatorFieldType.PlainText,
                Key = "username",
                Title = "Username",
                Placeholder = "Username"
            };

            FormAuthenticatorField password = new FormAuthenticatorField
            {
                FieldType = FormAuthenticatorFieldType.Password,
                Key = "password",
                Title = "Password",
                Placeholder = "Password"
            };

            Fields.Add(username);
            Fields.Add(password);
        }
    }
}
