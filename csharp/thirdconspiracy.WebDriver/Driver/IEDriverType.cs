using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace thirdconspiracy.WebDriver.Driver
{
    internal class IEDriverType : IDriverType
    {
        public IWebDriver GetLocalDriver()
        {
            // If you can't get the IE driver to work, open IE and select
            // Tools > Internet Options (switch to the "security" tab)
            // check "Enable Protected Mode (requires restarting Internet Explorer)
            var options = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true
            };
            var ie = new InternetExplorerDriver(options);
            ie.Manage().Cookies.DeleteAllCookies();

            return ie;
        }

        public IWebDriver GetRemoteDriver()
        {
            throw new NotImplementedException();
        }
    }
}
