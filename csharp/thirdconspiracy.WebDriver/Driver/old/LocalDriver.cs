using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;


namespace ChannelAdvisor.WebDriver.Driver
{
    public class LocalDriver : AbstractDriver
    {
        private readonly string _driverUrl = URL.driver;

        public override IWebDriver GetFirefoxDriver(string locale = "US")
        {
            //Eventually load profile based on locale
            //default locale is US
            locale = "en_US";

            var profile = FirefoxProfile(locale);

            return new FirefoxDriver(profile);
        }

        public override IWebDriver GetChromeDriver(string locale = "US")
        {
            //Eventually load profile based on locale
            //default locale is US
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.BinaryLocation = _driverUrl;
            return new ChromeDriver(options);
        }

        public override IWebDriver GetInternetExplorerDriver(string locale = "US")
        {
            //Eventually load profile based on locale
            //default locale is US

            var options = new InternetExplorerOptions { IntroduceInstabilityByIgnoringProtectedModeSettings = true };
            return new InternetExplorerDriver(_driverUrl, options);
        }
    }
}
