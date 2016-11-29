using Rubito.XamarinForms.SimpleAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubito.XamarinForms.SimpleAuth.Tests
{
    public class Helpers
    {
        public const string TokenEndpoint = "http://brentertainment.com/oauth2/lockdin/token";
        public const string Username = "demouser";
        public const string Password = "testpass";

        public static BearerToken GetValidToken()
        {
            return new BearerToken("TOKEN", "User", DateTime.Now, (DateTime.Now + new TimeSpan(1, 0, 0, 0)));
        }

        public static BearerToken GetExpiredToken()
        {
            return new BearerToken("TOKEN", "User", DateTime.Now, (DateTime.Now - new TimeSpan(1, 0, 0, 0)));
        }

        public static Credentials GetValidCredentials()
        {
            return new Credentials(Username, Password);
        }

        public static Credentials GetInvalidCredentials()
        {
            return new Credentials(Username, "456");
        }

        //public static ClientCredentialsAuthenticator GetAuthenticatorWithValidCredentials()
        //{
        //    return new ClientCredentialsAuthenticator(new Uri(TokenEndpoint), GetValidCredentials());
        //}

        //public static ClientCredentialsAuthenticator GetAuthenticatorWithInvalidCredentials()
        //{
        //    return new ClientCredentialsAuthenticator(new Uri(TokenEndpoint), GetInvalidCredentials());
        //}
    }
}
