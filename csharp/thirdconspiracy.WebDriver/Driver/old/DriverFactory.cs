using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Events;
using System.IO;
using Microsoft.VisualBasic;
using thirdconspiracy.WebDriver.Core;
using thirdconspiracy.WebDriver.Driver;

namespace ChannelAdvisor.WebDriver
{
    /// <summary>
    /// This is the driver factory that provides either local or remote web drivers.  Currently that includes Chrome, FireFox, and IE.
    /// </summary>
    public class DriverFactory
    {
        /// <summary>
        /// allow consumers to force to use the dev hub - false by default
        /// </summary>
        public bool UseDevSeleniumHub { get; set; }
        public string ProxyServer { get; set; }

        private readonly string downloadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\..\..\tmp";

        /// <summary>
        /// Initialize the Driver to setup several of the static variables and allows you to call browser later.
        /// </summary>
        public DriverFactory()
        {
            //Settings._assembly
            Console.Write("    -> Info: Adding 'core' and 'test' assembly reference... ");
            Settings.AddAssembly("core", Assembly.GetExecutingAssembly());
            Settings.AddAssembly("test", Assembly.GetCallingAssembly());
            Console.WriteLine("complete");

            var useGrid = Regex.IsMatch(Environment.MachineName, "(inttest|svc|appweb)", RegexOptions.IgnoreCase);
            //To run against DevSelenium, comment out the above useGrid line and uncomment both lines below
            //UseDevSeleniumHub = true;
            //var useGrid = true;

            Settings.isLocal = !useGrid;
            Settings.runEnv = Settings.GetCurrentEnvironment();
            Console.WriteLine("    -> Info: Settings.runEnv = {0}", Settings.runEnv);

            //Default browser download directory
            if (Directory.Exists(downloadPath)) return;
            Console.WriteLine("    -> Info: Creating default download folder\n   " + downloadPath);
            Directory.CreateDirectory(downloadPath);
        }

        /// <summary>
        /// The currently supported browsers.
        /// </summary>
        public enum BrowserType
        {
            Chrome,
            Firefox,
            Edge,
            InternetExplorer
        }

        public BrowserType WeeklyBrowserType()
        {
            //For now, always returning Chrome
            return BrowserType.Chrome;

            var dfi = DateTimeFormatInfo.CurrentInfo;
            var cal = dfi.Calendar;
            var weekOfYear = cal.GetWeekOfYear(DateTime.UtcNow, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            switch (weekOfYear % 4)
            {
                case 0:
                    return BrowserType.Chrome;
                case 1:
                    return BrowserType.Firefox;
                case 2:
                    return BrowserType.Edge;
                case 3:
                    return BrowserType.InternetExplorer;
            }
            throw new Exception(String.Format("Couldn't get browser for {0}", weekOfYear % 4));
        }

        /// <summary>
        /// This is the main way of getting and assigning a browser for your test to use.
        /// </summary>
        /// <param name="type">The enum of the browser you want to use</param>
        /// <returns>Returns a local browser object if running locally.  Otherwise uses the hub/node grid.</returns>
        public IWebDriver GetDriver(BrowserType type)
        {
            IWebDriver driver;
            switch (type)
            {
                case BrowserType.Firefox:
                    driver = GetFirefoxDriver();
                    break;
                case BrowserType.Chrome:
                    driver = GetChromeDriver();
                    break;
                case BrowserType.Edge:
                    driver = GetEdgeDriver();
                    break;
                case BrowserType.InternetExplorer:
                    driver = GetInternetExplorerDriver();
                    break;
                default:
                    driver = GetChromeDriver();
                    break;
            }

            if (driver == null)
                return null;

            if (type != BrowserType.Chrome)
                driver.Manage().Window.Maximize();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Constants.ImmediateTimeout);
            //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Constants.ImmediateTimeout));

            //driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(Constants.DefaultTimeout));
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(Constants.DefaultTimeout);
            return driver;
        }

        private IWebDriver GetEdgeDriver()
        {
            IWebDriver d;

            if (Settings.isLocal)
            {
                EdgeOptions options = new EdgeOptions();

                d = new EdgeDriver(URL.driver, options);
                return d;
            }
            else
            {
                DesiredCapabilities capabilities = DesiredCapabilities.Edge();
                var uri = new Uri(URL.devSeleniumHub);
                d = new RemoteWebDriverPlus(uri, capabilities, TimeSpan.FromSeconds(120));
                return d;
            }
        }

        public string getDriverVersion(IWebDriver driver)
        {
            IHasCapabilities capabilitiesDriver = driver as IHasCapabilities;
            return capabilitiesDriver.Capabilities.Version.ToString();
        }

        /// <summary>
        /// TODO: Set as Private
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public IWebDriver GetFirefoxDriver(string locale = "", FirefoxProfile profile = null)
        {
            Console.Write("Loading FireFox Driver");
            if (locale == "")
                locale = "en_US";
            IWebDriver d;

            if (profile == null)
            {
                profile = new FirefoxProfile();
                profile.AcceptUntrustedCertificates = true;
                profile.AssumeUntrustedCertificateIssuer = true;
            }

            if (Settings.isLocal)
            {
                FirefoxOptions options = new FirefoxOptions();
                options.Profile = profile;
                options.AddAdditionalCapability(CapabilityType.AcceptSslCertificates, true);
                options.AddAdditionalCapability(CapabilityType.HasNativeEvents, true);
                options.AddAdditionalCapability("acceptSslCerts", true);
                options.AddAdditionalCapability("acceptInsecureCerts", true);
                options.SetLoggingPreference(LogType.Browser.ToString(), LogLevel.All);
                options.LogLevel = FirefoxDriverLogLevel.Trace;

                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(URL.driver);
                d = new FirefoxDriver(service, options, TimeSpan.FromMinutes(60));
                
                return d;
            }
            else
            {
                DesiredCapabilities desiredCapabilities = DesiredCapabilities.Firefox();
                desiredCapabilities.SetCapability("firefox_profile", profile);
                d = new RemoteWebDriverPlus(new Uri(URL.seleniumHub), desiredCapabilities);
                return d;
            }
        }

        /// <summary>
        /// returns a Chrome driver
        /// </summary>
        /// <returns></returns>
        private IWebDriver GetChromeDriver()
        {
            Console.Write("Loading Chrome Driver");
            IWebDriver d;
            ChromeOptions options = setStandardChromeOptions();

            if (Settings.isLocal)
            {
                d = new ChromeDriver(URL.driver, options);
                Console.WriteLine(", Chrome Version: " + getDriverVersion(d));
                return d;
            }

            //if not local, then connect through the hub
            int connectionLimit = 3;
            DesiredCapabilities capability = (DesiredCapabilities)options.ToCapabilities();
            
            for (int connectionAttempts = 0; connectionAttempts < connectionLimit; connectionAttempts++)
            {
                Console.Write("-> Connection Attempt: " + (connectionAttempts + 1));
                try
                {
                    if (UseDevSeleniumHub)
                    {
                        Console.WriteLine(" Running against the devSeleniumHub");
                        var uri = new Uri(URL.devSeleniumHub);
                        d = new RemoteWebDriverPlus(uri, capability, TimeSpan.FromSeconds(120));
                    }
                    else
                    {
                        Console.WriteLine(" Running against the SeleniumHub");
                        d = new RemoteWebDriverPlus(new Uri(URL.seleniumHub), capability, TimeSpan.FromSeconds(120));
                    }
                    Console.WriteLine("-> Connection established");
                    Console.WriteLine("Chrome Version: " + getDriverVersion(d));
                    return d;
                }
                catch (Exception e)
                {
                    Console.WriteLine(" -> Error thrown. Retrying.");
                    Thread.Sleep(2500);
                    if (connectionAttempts == connectionLimit - 1)
                    {
                        Console.WriteLine("-> Connection Failed");
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
            }

            return null;
        }

        private ChromeOptions setStandardChromeOptions()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("start-maximized");
            options.AddArguments("--disable-blink-features=BlockCredentialedSubresources");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            if (!string.IsNullOrEmpty(ProxyServer))
            {
                options.AddArgument("--proxy-server=" + ProxyServer);
            }

            return options;
        }

        /// <summary>
        /// returns an Internet Explorer driver
        /// </summary>
        /// <returns></returns>
        private IWebDriver GetInternetExplorerDriver()
        {
            Console.Write("Loading IE Driver");
            IWebDriver d;

            if (Settings.isLocal)
            {
                /*
                 * If you can't get the IE driver to work, open IE and select
                 * Tools > Internet Options (switch to the "security" tab)
                 * check "Enable Protected Mode (requires restarting Internet Explorer)
                */
                InternetExplorerOptions options = new InternetExplorerOptions();
                options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                d = new InternetExplorerDriver(URL.driver, options);
            }
            else
            {
                DesiredCapabilities capabilities = DesiredCapabilities.InternetExplorer();
                capabilities.SetCapability(CapabilityType.AcceptSslCertificates, true);
                capabilities.SetCapability(CapabilityType.HandlesAlerts, true);
                capabilities.SetCapability("ignoreProtectedModeSettings", true);
                d = new RemoteWebDriverPlus(new Uri(URL.seleniumHub), capabilities);
            }
            d.Manage().Cookies.DeleteAllCookies();
            return d;
        }

        /// <summary>
        /// This method randomly selects a (enum)browser type and then calls the GetDriver(type) method.
        /// 
        /// Example Usage: IWebDriver driver = d.GetRandomDriver();
        /// </summary>
        /// <returns>Random IWebDriver</returns>
        public IWebDriver GetRandomDriver()
        {
            var browserType = WeeklyBrowserType();
            return GetDriver(browserType);
        }

        public void firingDriver_ExceptionThrown(object sender, WebDriverExceptionEventArgs e)
        {
            ScreenShot.SaveScreenShot(e.Driver, @"..\..\..\Img\exception.jpeg");
        }


    }
}
