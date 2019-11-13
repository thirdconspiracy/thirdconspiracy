using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using thirdconspiracy.WebDriver.Core;
using thirdconspiracy.WebDriver.Helpers;

namespace thirdconspiracy.WebDriver.IntegrationTests
{
    [TestFixture]
    public class WebDriverTestBase
    {
        protected IWebDriver _driver;


        [TearDown]
        public void TestFinally()
        {
            if (TestContext.CurrentContext.Result.Outcome == ResultState.Error ||
                TestContext.CurrentContext.Result.Outcome == ResultState.Failure)
            {
                ScreenShot.SaveImage(null);
                ScreenShot.SaveSource(null);
            }
            _driver?.Close();
        }
    }
}
