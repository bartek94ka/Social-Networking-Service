using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace LocalSocial.Tests.Views
{
    [TestFixture]
    internal class RegisterViewTests
    {
        [Test]
        public void RegisterView_NoThrowException()
        {
            //Arrange
            var webDriver = (IWebDriver)new FirefoxDriver();
            webDriver.Navigate().GoToUrl(@"http://localsocial.azurewebsites.net/#/register");
            var webElements = webDriver.FindElements(By.TagName("input"));
        }
    }
}
