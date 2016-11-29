using System;
using System.Net.Http.Headers;

namespace Rubito.XamarinForms.SimpleAuth.Models
{
    public class BearerToken
    {
        public string AccessToken { get; private set; }

        public string UserName { get; private set; }

        public DateTime Issued { get; private set; }

        public DateTime Expires { get; private set; }

        public BearerToken(string accessToken, string userName, DateTime issued, DateTime expires)
        {
            this.AccessToken = accessToken;
            this.UserName = userName;
            this.Issued = issued;
            this.Expires = expires;
        }

        public AuthenticationHeaderValue ToAuthenticationHeaderValue()
        {
            return new AuthenticationHeaderValue("Bearer", AccessToken);
        }

        public bool IsExpired()
        {
            return (DateTime.Today.CompareTo(Expires) > 0);
        }

        public override string ToString()
        {
            return $"Bearer {AccessToken}";
        }
    }
}
