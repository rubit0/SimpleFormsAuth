using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rubito.XamarinForms.SimpleAuth.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rubito.XamarinForms.SimpleAuth
{
    public class ClientCredentialsAuthenticator
    {
        public Uri TokenEndpoint { get; }
        private readonly Credentials _credentials;

        public ClientCredentialsAuthenticator(Uri tokenEndpoint, Credentials credentials)
        {
            if (tokenEndpoint == null || credentials == null)
                throw new ArgumentNullException();

            TokenEndpoint = tokenEndpoint;
            _credentials = credentials;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(HttpMessageHandler httpMessageHandler = null)
        {
            var client = (httpMessageHandler != null) ? new HttpClient(httpMessageHandler) : new HttpClient();
            using (client)
            {
                var response = await client.PostAsync(TokenEndpoint, _credentials.ToFormUrlEncodedContent());
                var result = await HttpResponseValidationAsync(response);

                return result;
            }
        }

        private async Task<AuthenticationResult> HttpResponseValidationAsync(HttpResponseMessage response)
        {
            var contentStream = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var token = await Task.Factory
                    .StartNew(() => JsonConvert.DeserializeObject<BearerToken>(contentStream));

                return new AuthenticationResult(true, "Success", token);
            }

            var message = await Task.Factory
                .StartNew(() => JsonConvert.DeserializeObject<JObject>(contentStream));

            var errorMessage = "Error";
            if (message["error_description"] != null)
                errorMessage = (string)message["error_description"];

            return new AuthenticationResult(false, errorMessage);
        }
    }
}
