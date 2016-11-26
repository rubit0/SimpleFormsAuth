using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubito.XamarinForms.SimpleAuth.Models;
using System;

namespace Rubito.XamarinForms.SimpleAuth.Tests
{
    [TestClass]
    public class TokenTests
    {
        [TestMethod]
        public void TokenIsInvalidOnExpiredDate()
        {
            var token = Helpers.GetExpiredToken();

            Assert.IsTrue(token.IsExpired());
        }

        [TestMethod]
        public void TokenIsValidOnFutureDate()
        {
            var token = Helpers.GetValidToken();

            Assert.IsFalse(token.IsExpired());
        }

        [TestMethod]
        public void TokenToAuthenticationHeaderIsNotNull()
        {
            var token = Helpers.GetValidToken();
            var authHeader = token.ToAuthenticationHeaderValue();

            Assert.IsNotNull(authHeader);
        }

        [TestMethod]
        public void AuthenticationHeaderFromTokenIsEqual()
        {
            var token = Helpers.GetValidToken();
            var authHeader = token.ToAuthenticationHeaderValue();

            var result= string.Compare(token.ToString(), authHeader.ToString()) == 0;
            Assert.IsTrue(result);
        }
    }
}
