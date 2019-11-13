using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Events;
using thirdconspiracy.WebDriver.Constants;
using thirdconspiracy.WebDriver.Core;
using thirdconspiracy.WebDriver.Helpers;

namespace thirdconspiracy.WebDriver.Driver
{
    public class DriverFactory
    {
        #region Member Variables

        private readonly BrowserType _type;
        private readonly bool _isLocal;

        #endregion Member Variables

        #region CTOR

        public DriverFactory(BrowserType type)
        {
            _type = type;
            //TODO: Grid
            // * Grid IP
            // * Determine when "debug/local"
            _isLocal = true;
        }

        public DriverFactory() : this(WeeklyBrowserType())
        { }

        private static BrowserType WeeklyBrowserType()
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var cal = dfi.Calendar;
            var weekOfYear = cal.GetWeekOfYear(DateTime.UtcNow, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            var browserWeek = weekOfYear % 4;
            switch (browserWeek)
            {
                case 0:
                    return BrowserType.Chrome;
                case 1:
                    return BrowserType.Firefox;
                case 2:
                    return BrowserType.Edge;
                case 3:
                    return BrowserType.InternetExplorer;
                default:
                    throw new Exception($"Couldn't get browser for {browserWeek}");
            }
        }

        #endregion CTOR

        /// <summary>
        /// This is the main way of getting and assigning a browser for your test to use.
        /// </summary>
        /// <param name="type">The enum of the browser you want to use</param>
        /// <returns>Returns a local browser object if running locally.  Otherwise uses the hub/node grid.</returns>
        public IWebDriver GetDriver()
        {
            
            IDriverType driverType;
            switch (_type)
            {
                case BrowserType.Firefox:
                    throw new NotImplementedException();
                case BrowserType.Edge:
                    driverType = new EdgeDriverType();
                    break;
                case BrowserType.InternetExplorer:
                    driverType = new IEDriverType();
                    break;
                case BrowserType.Chrome:
                default:
                    driverType = new ChromeDriverType();
                    break;
            }

            var driver = _isLocal
                ? driverType.GetLocalDriver()
                : driverType.GetRemoteDriver();

            if (driver == null)
                return null;

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(WebDriverConstants.Timeout.ImmediateTimeout);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(WebDriverConstants.Timeout.DefaultTimeout);
            var eventDriver = new EventFiringWebDriver(driver);
            eventDriver.ExceptionThrown += ScreenShot.DriverExceptionEvent;

            return driver;
        }

    }
}
