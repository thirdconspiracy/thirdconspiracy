using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace ChannelAdvisor.WebDriver.Driver
{
    public class RemoteDriver : AbstractDriver
    {
        private static Uri Hub { get { return new Uri(URL.seleniumHub); } }
        //private static Uri Hub { get { return new Uri(URL.devSeleniumHub); } }

        public override IWebDriver GetFirefoxDriver(string locale = "US")
        {

            var desiredCapabilities = SetCapabilities(DesiredCapabilities.Firefox());
            var profile = FirefoxProfile(locale);
            desiredCapabilities.SetCapability(FirefoxDriver.ProfileCapabilityName, profile);
            return new ScreenShotRemoteWebDriver(Hub, desiredCapabilities);
        }

        public override IWebDriver GetChromeDriver(string locale = "US")
        {
            var desiredCapabilities = SetCapabilities(DesiredCapabilities.Chrome());
            return new ScreenShotRemoteWebDriver(Hub, desiredCapabilities);
        }

        public override IWebDriver GetInternetExplorerDriver(string locale = "US")
        {
            var desiredCapabilities = SetCapabilities(DesiredCapabilities.InternetExplorer());
            desiredCapabilities.SetCapability("ignoreProtectedModeSettings", true);
            return new ScreenShotRemoteWebDriver(Hub, desiredCapabilities);
        }

    }
}

