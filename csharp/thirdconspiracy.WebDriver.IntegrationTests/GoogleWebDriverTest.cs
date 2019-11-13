using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using thirdconspiracy.WebDriver.Constants;
using thirdconspiracy.WebDriver.Driver;

namespace thirdconspiracy.WebDriver.IntegrationTests
{
    [TestFixture]
    public class GoogleWebDriverTest : WebDriverTestBase
    {
        [SetUp]
        public void Setup()
        {
            _driver = new DriverFactory(BrowserType.Chrome).GetDriver();
            _driver.Navigate().GoToUrl("https://www.google.com");
        }

        [Test, Explicit]
        public void TestScreenCaptureAssertFail()
        {
            Assert.Fail();
        }

        [Test, Explicit]
        public void TestScreenCaptureDriverException()
        {
            var element = _driver.FindElementSafe(By.Id("..."));
            element.Click();
        }

        [Test, Explicit]
        public void TestSearch()
        {
            throw new NotImplementedException();
        }

    }
}
