using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubito.XamarinForms.SimpleAuth.Models;
using System;

namespace Rubito.XamarinForms.SimpleAuth.Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        [TestMethod]
        public void TokenIsInvalidOnExpiredDate()
        {
            var token = new BearerToken("TOKEN", "User", DateTime.Now, (DateTime.Now - new TimeSpan(1, 0, 0, 0)));

            Assert.IsTrue(token.IsExpired());
        }
    }
}
