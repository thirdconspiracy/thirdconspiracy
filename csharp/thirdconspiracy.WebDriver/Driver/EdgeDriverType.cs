using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using thirdconspiracy.WebDriver.Constants;

namespace thirdconspiracy.WebDriver.Driver
{
    internal class EdgeDriverType : IDriverType
    {
        public IWebDriver GetLocalDriver()
        {
            var options = new EdgeOptions();
            var edgeDriver = new EdgeDriver(WebDriverConstants.LocalPath.Driver, options);

            return edgeDriver;
        }

        public IWebDriver GetRemoteDriver()
        {
            throw new NotImplementedException();
        }
    }
}
