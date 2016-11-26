using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rubito.XamarinForms.SimpleAuth.Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        [TestMethod]
        public void AuthenticationResultIsNotNull()
        {
            var authenticator = Helpers.GetAuthenticatorWithValidCredentials();
            var result = authenticator.AuthenticateAsync().Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AuthenticationResultIsFailedOnInvalidCredentials()
        {
            var authenticator = Helpers.GetAuthenticatorWithInvalidCredentials();
            var result = authenticator.AuthenticateAsync().Result;

            Assert.IsFalse(result.IsAuthenticated);
        }

        [TestMethod]
        public void AuthenticationResultIsValidOnValidCredentials()
        {
            var authenticator = Helpers.GetAuthenticatorWithValidCredentials();
            var result = authenticator.AuthenticateAsync().Result;

            Assert.IsFalse(result.IsAuthenticated);
        }

        [TestMethod]
        public void TokenIsNotNullOnValidAuthentication()
        {
            var authenticator = Helpers.GetAuthenticatorWithValidCredentials();
            var result = authenticator.AuthenticateAsync().Result;

            if(result.IsAuthenticated)
                Assert.IsNotNull(result.Token);
        }
    }
}
