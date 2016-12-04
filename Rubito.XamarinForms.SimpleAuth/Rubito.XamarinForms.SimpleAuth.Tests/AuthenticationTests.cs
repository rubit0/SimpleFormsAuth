using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rubito.XamarinForms.SimpleAuth.Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsExeptionOnNullUri()
        {
            var auth = new OAuth2PasswordCredentialsAuthenticator(null);
        }

        [TestMethod]
        public void BasicFailedAuthenticationFlow()
        {
            var auth = new OAuth2PasswordCredentialsAuthenticator(new Uri("http://www.google.com"));
            auth.SetCredentials("Bob", "strongedpassword");
            var errorFired = false;

            auth.Error += (obj, args) => errorFired = true;
            auth.Completed += (obj, args) => Assert.Fail("Complete should not be rised on failure");

            var result = auth.SignInAsync().Result;

            Assert.IsFalse(auth.HasCompleted);
            Assert.IsTrue(errorFired);
        }

        [TestMethod]
        public void BasicValidAuthenticationFlow()
        {
            var completeFired = false;
            var tokenEndpoint = new Uri("https://companistawebtesting.azurewebsites.net/Token");

            var auth = new OAuth2PasswordCredentialsAuthenticator(tokenEndpoint);
            auth.SetCredentials("Bill", "Abc_123");
            auth.Error += (obj, args) => Assert.Fail("Error should not be rised on success");
            auth.Completed += (obj, args) => completeFired = args.IsAuthenticated;


            var account = auth.SignInAsync().Result;

            Assert.AreEqual(tokenEndpoint, auth.AccessTokenUrl);
            Assert.IsTrue(auth.HasCompleted);
            Assert.IsTrue(completeFired);
            Assert.IsNotNull(account);
        }

        [TestMethod]
        public void AuthenticatorConstructorWithCredentialsGeneratesFields()
        {
            var tokenEndpoint = new Uri("https://companistawebtesting.azurewebsites.net/Token");
            var auth = new OAuth2PasswordCredentialsAuthenticator(tokenEndpoint);
            auth.SetCredentials("Bill", "Abc_123");

            Assert.IsTrue(auth.Fields.Count > 1);
        }

        [TestMethod]
        public void ExceptionMessageIsNotNull()
        {
            var auth = new OAuth2PasswordCredentialsAuthenticator(new Uri("https://Google.com"));
            auth.SetCredentials("Bill", "Abc_123");

            var completeFired = false;
            var message = string.Empty;

            auth.Error += (obj, args) =>
            {
                completeFired = true;
                message = args.Message;
            };

            var account = auth.SignInAsync().Result;

            Assert.IsTrue(completeFired);
            Assert.IsNotNull(message);
        }
    }
}
