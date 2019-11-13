using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace ChannelAdvisor.WebDriver.Driver
{
    public abstract class AbstractDriver
    {
        abstract public IWebDriver GetFirefoxDriver(string locale = "US");
        abstract public IWebDriver GetChromeDriver(string locale = "US");
        abstract public IWebDriver GetInternetExplorerDriver(string locale = "US");

        private readonly string _downloadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\..\..\tmp";
        protected FirefoxProfile FirefoxProfile(string locale)
        {
            var profile = new FirefoxProfile();
            profile.SetPreference("browser.download.dir", _downloadPath);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/save, application/text, application/vnd.ms-excel, text/plain, application/csv, text/csv");
            //profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/force-download");
            //firefoxProfile.SetPreference("browser.helperApps.alwaysAsk.force", false);
            profile.SetPreference("browser.download.useDownloadDir", "false");
            //profile.SetPreference("browser.download.useDownloadDir", true);
            profile.SetPreference("browser.download.folderList", 2);
            //profile.SetPreference("browser.download.folderList", 0);
            profile.SetPreference("intl.accept_languages", locale);
            return profile;
            //firefoxProfile.SetPreference("browser.download.manager.showWhenStarting", false);
            //firefoxProfile.SetPreference("browser.download.manager.focusWhenStarting", false);
            //firefoxProfile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
            //firefoxProfile.SetPreference("browser.download.manager.closeWhenDone", true);
            //firefoxProfile.SetPreference("browser.download.manager.showAlertOnComplete", false);
            //firefoxProfile.SetPreference("browser.download.manager.useWindow", false);
        }

        protected static DesiredCapabilities SetCapabilities(DesiredCapabilities desiredCapabilities)
        {
            desiredCapabilities.SetCapability(CapabilityType.AcceptSslCertificates, true);
            desiredCapabilities.SetCapability("acceptSslCerts", "true");
            desiredCapabilities.SetCapability(CapabilityType.HandlesAlerts, true);
            desiredCapabilities.SetCapability("resolution", "1280x1024");

            return desiredCapabilities;
        }
    }
}
