using ModernHttpClient;
using Rubito.XamarinForms.SimpleAuth.Pages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Utilities;

namespace Rubito.XamarinForms.SimpleAuth
{
    public class OAuth2PasswordCredentialsAuthenticator : FormAuthenticator
    {
        string _scope;
        Uri _accessTokenUrl;

        public Uri AccessTokenUrl
        {
            get { return this._accessTokenUrl; }
        }

        public OAuth2PasswordCredentialsAuthenticator(Uri accessTokenUrl, Uri createAccountLink = null, string scope = null) : base(createAccountLink)
        {
            if (accessTokenUrl == null)
                throw new ArgumentNullException(nameof(accessTokenUrl), "an accessTokenUrl must be provided");

            this._accessTokenUrl = accessTokenUrl;
            this._scope = scope;

            InitializeFormFields();
        }

        public OAuth2PasswordCredentialsAuthenticator(Uri accessTokenUrl, string username, string password, Uri createAccountLink = null, string scope = null) : base(createAccountLink)
        {
            if (accessTokenUrl == null)
                throw new ArgumentNullException(nameof(accessTokenUrl), "an accessTokenUrl must be provided");

            this._accessTokenUrl = accessTokenUrl;
            this._scope = scope;

            InitializeFormFields(username, password);
        }

        public virtual void InitializeFormFields(string username = "", string password = "", bool isEmail = false)
        {
            if (string.IsNullOrEmpty(GetFieldValue("username")))
            {
                var usernameField = new FormAuthenticatorField
                {
                    FieldType = (isEmail)
                        ? FormAuthenticatorFieldType.Email
                        : FormAuthenticatorFieldType.PlainText,
                    Key = "username",
                    Title = (isEmail)
                        ? "Email"
                        : "Username",
                    Placeholder = "Username",
                    Value = username
                };

                Fields.Add(usernameField);
            }

            if (string.IsNullOrEmpty(GetFieldValue("password")))
            {
                var passwordField = new FormAuthenticatorField
                {
                    FieldType = FormAuthenticatorFieldType.Password,
                    Key = "password",
                    Title = "Password",
                    Placeholder = "Password",
                    Value = password
                };

                Fields.Add(passwordField);
            }
        }

        public override async Task<Account> SignInAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (cancellationToken.IsCancellationRequested && AllowCancel)
            {
                OnCancelled();
                cancellationToken.ThrowIfCancellationRequested();
            }

            try
            {
                var result = await RequestAccessTokenAsync(cancellationToken);

                if (result.ContainsKey("error"))
                    OnError(result["error_description"]);

                if (result.ContainsKey("access_token"))
                {
                    var account = new Account(result["userName"], result);
                    OnSucceeded(account);
                    return account;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cancellationToken)
                    OnError("User aborted the sign-in process or possible time out");

                return null;
            }
            catch (AuthException ex)
            {
                OnError(ex.Message);
                return null;
            }

            return null;
        }

        public virtual ContentPage GetFormsUI()
        {
            return new AuthDialogPage(this);
        }

        protected async Task<IDictionary<string, string>> RequestAccessTokenAsync(CancellationToken cancellationToken)
        {
            var query = FieldsToFormUrlEncodedContent();

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                var response = await client.PostAsync(_accessTokenUrl, query, cancellationToken);
                var contentStream = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (response.Content.Headers.ContentType.MediaType.Contains("html"))
                        throw new AuthException($"{response.StatusCode}: {response.ReasonPhrase}");
                }

                var data = contentStream.Contains("{")
                        ? WebEx.JsonDecode(contentStream)
                        : WebEx.FormDecode(contentStream);

                if (data.ContainsKey("access_token") || data.ContainsKey("error"))
                    return data;

                throw new AuthException("Expected an access_token or error header in the response message.");
            }
        }

        protected virtual FormUrlEncodedContent FieldsToFormUrlEncodedContent()
        {
            if (Fields.Count < 2)
                this.InitializeFormFields();

            var credentialsEncoded = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", GetFieldValue("username")},
                {"password", GetFieldValue("password")}
            };

            if (!string.IsNullOrEmpty(_scope))
                credentialsEncoded.Add("scope", _scope);

            return new FormUrlEncodedContent(credentialsEncoded);
        }
    }
}
