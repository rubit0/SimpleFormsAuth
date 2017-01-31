using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Auth;

namespace Rubito.SimpleFormsAuth
{
    public class OAuth2BearerRequest : Request
    {
        public HttpContent RequestBodyContent { get; protected set; }
        bool _acceptJson = true;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="method">The HTTP Method.</param>
        /// <param name="url">The target URL endpoint.</param>
        /// <param name="parameters">Parameters that will be populated in the requests.</param>
        /// <param name="account">Account that is used to authenticate the requests.</param>
        /// <param name="acceptJson">Accept JSON as MediaType.</param>
        public OAuth2BearerRequest(string method, Uri url, IDictionary<string, string> parameters = null, Account account = null, bool acceptJson = true) : base(method, url, parameters, account)
        {
            _acceptJson = acceptJson;
        }

        /// <summary>
        /// Set the HttpContent body for this request object.
        /// The content type is set by default to 'application/json'
        /// </summary>
        /// <param name="body">Request body represented as a string</param>
        /// <param name="contentType">The content type for this request.</param>
        public virtual void SetRequestBody(string body, string contentType = "application/json")
        {
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body), "you must provice a body");

            this.RequestBodyContent = new StringContent(body, Encoding.UTF8, contentType);
        }

        protected override HttpRequestMessage GetPreparedWebRequest()
        {
            var request = base.GetPreparedWebRequest();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", base.Account.Properties["access_token"]);

            if (this.RequestBodyContent != null)
                request.Content = this.RequestBodyContent;

            if (_acceptJson)
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return request;
        }
    }
}
