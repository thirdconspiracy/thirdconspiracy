using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace thirdconspiracy.WebDriver.Driver
{
    internal class ChromeDriverType : IDriverType
    {
        public IWebDriver GetLocalDriver()
        {
            var driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            return driver;
        }

        public IWebDriver GetRemoteDriver()
        {
            throw new NotImplementedException();
        }
    }
}
