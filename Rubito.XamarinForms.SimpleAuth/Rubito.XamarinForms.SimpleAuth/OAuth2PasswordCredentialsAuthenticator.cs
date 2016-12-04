using ModernHttpClient;
using Rubito.XamarinForms.SimpleAuth.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Create a new authenticator instance, you need to provide a link to the token endpoint.
        /// </summary>
        /// <param name="accessTokenUrl">Request endpoint to retrieve a bearer token</param>
        /// <param name="createAccountLink">Link for creating an account. Currently ignored.</param>
        /// <param name="scope">Optional scope for the token request</param>
        public OAuth2PasswordCredentialsAuthenticator(Uri accessTokenUrl, Uri createAccountLink = null, string scope = null) : base(createAccountLink)
        {
            if (accessTokenUrl == null)
                throw new ArgumentNullException(nameof(accessTokenUrl), "an accessTokenUrl must be provided");

            this._accessTokenUrl = accessTokenUrl;
            this._scope = scope;

            InitializeFormFields();
        }

        /// <summary>
        /// Set credentials for this authenticator and underlying dialog page.
        /// </summary>
        /// <param name="username">Set a username, eg. load from an Account if you have one</param>
        /// <param name="password">Password sent along the username</param>
        /// <param name="isEmail">Set the username to be treated and validates as an email address.</param>
        public virtual void SetCredentials(string username = "", string password = "", bool isEmail = false)
        {
            var userField = Fields.SingleOrDefault(f => f.Key == "username");
            var passwordField = Fields.SingleOrDefault(f => f.Key == "password");

            if (userField != null)
            {
                if (!string.IsNullOrEmpty(username))
                    userField.Value = username;

                userField.FieldType = (isEmail)
                    ? FormAuthenticatorFieldType.Email
                    : FormAuthenticatorFieldType.PlainText;

                userField.Title = (isEmail)
                    ? "Email"
                    : "Username";
                userField.Placeholder = (isEmail)
                    ? "Email"
                    : "Username";
            }

            if (passwordField != null)
            {
                passwordField.Value = password;
            }
            else if (!string.IsNullOrEmpty(password))
            {
                passwordField.Value = password;
            }
        }

        protected virtual void InitializeFormFields()
        {
            var userField = Fields.SingleOrDefault(f => f.Key == "username");
            var passwordField = Fields.SingleOrDefault(f => f.Key == "password");

            if (userField == null)
            {
                userField = new FormAuthenticatorField
                {
                    FieldType = FormAuthenticatorFieldType.PlainText,
                    Key = "username",
                    Title = "Username",
                    Placeholder = "Username",
                    Value = ""
                };

                Fields.Add(userField);
            }

            if (passwordField == null)
            {
                passwordField = new FormAuthenticatorField
                {
                    FieldType = FormAuthenticatorFieldType.Password,
                    Key = "password",
                    Title = "Password",
                    Placeholder = "Password",
                    Value = ""
                };

                Fields.Add(passwordField);
            }
        }

        /// <summary>
        /// Start authentication with the provided credentials.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>Get the Account to do wonderful things.</returns>
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

        /// <summary>
        /// Present this with love cradted page to your user as a modal page.
        /// Please note that this page closes it-self on a successful authentication.
        /// </summary>
        /// <returns>The authentication dialog page.</returns>
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
