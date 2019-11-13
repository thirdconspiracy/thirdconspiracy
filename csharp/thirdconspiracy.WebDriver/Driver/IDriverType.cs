using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace thirdconspiracy.WebDriver.Driver
{
    internal interface IDriverType
    {
        IWebDriver GetLocalDriver();
        IWebDriver GetRemoteDriver();
    }
}
