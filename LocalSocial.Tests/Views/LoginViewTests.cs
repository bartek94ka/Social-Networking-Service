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
    internal class LoginViewTests
    {
        [Test]
        public void LoginView_NoThrowException()
        {
            //Arrange
            var webDriver = (IWebDriver)new FirefoxDriver();
            webDriver.Navigate().GoToUrl(@"http://localsocial.azurewebsites.net/#/login");
            var webElements = webDriver.FindElements(By.TagName("input"));


            //Act
            webElements[0].SendKeys("user1@ls.pl");
            webElements[1].SendKeys("user1!");
            var button = webDriver.FindElement(By.TagName("button"));
            button.Click();
            //var action = new Action(() => {  });

            //Arrange
            //Should.NotThrow(action);
        }
    }
}
