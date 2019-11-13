using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using thirdconspiracy.WebDriver.Constants;

namespace thirdconspiracy.WebDriver.Driver
{
    public static class DriverExtensions
    {
        #region Search

        /// <summary>
        /// Same as FindElement only returns null when not found instead of an exception.
        /// </summary>
        /// <param name="driver">current browser instance</param>
        /// <param name="by">The search string for finding element</param>
        /// <returns>Returns element or null if not found</returns>
        public static IWebElement FindElementSafe(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (InvalidElementStateException e)
            {
                throw new Exception($"Bad selector - {@by}", e);
            }
        }

        #endregion Search

        /*
        // www.vcskicks.com/selenium-extensions.php
        public static string GetText(this IWebDriver driver)
        {
            return driver.FindElement(By.TagName("body")).Text;
        }

        public static void NavigateTo(this IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
            WaitForPageReady(driver);
        }

        #endregion Search

        #region Click

        /// <summary>
        /// Allows you to run a javascript .click() on an IWebElement when it is not visible in the UI
        /// </summary>
        /// <author>Brantley Blanchard</author>
        /// <param name="elementToClick">This is the current element you would like to click</param>
        /// <param name="driver">This is the browser object that is displaying the elment you would like to click</param>
        /// <returns>Always returns true</returns>
        public static bool InvisibleClick(this IWebDriver driver, IWebElement elementToClick)
        {
            bool exists = elementToClick.Exists();
            if (exists)
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click()", elementToClick);
                driver.WaitForPageReady();
            }
            else
            {
                Console.WriteLine("Link was not found to InvisibleClick");
            }
            return exists;
        }

        public static void DependableClick(this IWebDriver driver, IWebElement elementToClick, bool beforeWaitForDOM = false, bool afterWaitForDOM = true)
        {
            if (elementToClick == null)
            {
                throw new NoSuchElementException("Element is not initialized.");
            }

            if (beforeWaitForDOM == true)
            {
                WaitForPageReady(driver);
            }

            try
            {
                elementToClick.Click();
            }
            catch (ElementNotVisibleException)
            {
                elementToClick.InvisibleClick(driver);
            }
            catch
            {
                throw new Exception("Element not found.");
            }

            if (afterWaitForDOM == true)
            {
                WaitForPageReady(driver);
            }
        }

        public static void DependableDoubleClick(this IWebDriver driver, IWebElement element, bool beforeWaitForDOM = false, bool afterWaitForDOM = true)
        {
            if (element == null)
            {
                throw new NoSuchElementException("Element is not initialized");
            }

            if (beforeWaitForDOM == true)
            {
                WaitForPageReady(driver);
            }

            try
            {
                const string script = "var evt = document.createEvent('MouseEvents');" +
                                      "evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0,null);" +
                                      "arguments[0].dispatchEvent(evt);";
                ((IJavaScriptExecutor)driver).ExecuteScript(script, element);
            }
            catch
            {
                throw new Exception("Unable to doubleclick element.");
            }

            if (afterWaitForDOM == true)
            {
                WaitForPageReady(driver);
            }
        }

        public static void AlertAccept(this IWebDriver driver)
        {
            driver.SwitchTo().Alert().Accept();
        }

        #endregion Click

        #region Loading

        public static bool IsSiteOk(string url)
        {
            bool ok;
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(url);
                ServicePointManager.DefaultConnectionLimit = 96;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine("{0} is {1}", url, response.StatusCode);
                ok = true;
            }
            catch (System.Net.WebException e)
            {
                Console.WriteLine("{0}", e.Message);
                ok = false;
            }
            return ok;
        }

        public static bool WaitForPageReady(this IWebDriver driver, int msHardPause = 500, int msDomIsQuietAfter = 1000)
        {

            bool isQuiet = false;

            var timeout = new TimeSpan(0, 0, WebDriverConstants.AkamaiDefaultTimeout);
            var wait = new WebDriverWait(driver, timeout);

            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("driver", "Driver must support javascript execution");
            }

            isQuiet = wait.Until<bool>((d) =>
            {
                // d is driver
                try
                {
                    // Poll for observer
                    bool observerExists = Boolean.Parse(javascript.ExecuteScript(
                        "if (window.automationQuietnessObserver) { return true; } else { return false; }"
                        ).ToString());
                    // Check if mutationobserver exists
                    if (!observerExists)
                    {
                        // False => inject mutationobserver and thread sleep msHardPause
                        javascript.ExecuteScript(
                            // Describe the observer - every time there is a mutation lets set a timestamp
                            "window.automationQuietnessObserver = new MutationObserver(function(mutations) {" +
                            "mutations.forEach(function(mutation) {" +
                            "window.automationLastMutation = new Date();" + // Timestamp the mutation
                            "});" +
                            "});" +
                            // Describe the config (ie what to listen for) - listen for everything
                            "window.automationObserverConfig = {" +
                            "attributes: true," +
                            "childList: true," +
                            "characterData: true," +
                            "subtree: true" +
                            "};" +
                            // Default the mutation timestamp if no timestamp yet this will ensure that there is a legit wait after
                            "window.automationLastMutation = new Date();" +
                            // Start the observer watching the body for everything
                            "window.automationQuietnessObserver.observe(document.body,window.automationObserverConfig);"
                            );
                        Thread.Sleep(msHardPause);
                    }

                    // Poll diff in time of last mutation and current time
                    int msFromLastMutation = int.Parse(javascript.ExecuteScript(
                        "if(window.automationLastMutation && Object.prototype.toString.call(window.automationLastMutation) === '[object Date]') {" +
                        "return Math.abs(window.automationLastMutation.getTime() - (new Date()).getTime());" +
                        "}" +
                        "else {" +
                        "return 0;" +
                        "}"
                        ).ToString());

                    // Check if time diff between last mutation and current time is greater than msDomIsQuiet
                    if (msFromLastMutation >= msDomIsQuietAfter)
                    {
                        // True => validate DOM is in readystate = complete
                        if (!IsDomInReadyState(d))
                        {
                            return false;
                        }
                        // True => validate DOM is not paused
                        if (IsAutomationPaused(d))
                        {
                            return false;
                        }
                        // True => stop observing set isQuiet = true and exit loop
                        javascript.ExecuteScript(
                            "if(window.automationQuietnessObserver) {" +
                            "window.automationQuietnessObserver.disconnect();" +
                            "window.automationQuietnessObserver = null;" +
                            "window.automationLastMutation = null;" +
                            "}"
                            );
                        return true;
                    }
                    return false;
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available; eat it???
                    return false;
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available; eat it???
                    return false;
                }
                catch (Exception)
                {
                    // Something else went wrong; eat it???
                    return false;
                }
            });

            return isQuiet;
        }
        private static bool IsDomInReadyState(IWebDriver driver, string expectedReadyState = "complete")
        {
            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("driver", "Driver must support javascript execution");
            }
            string readyState = javascript.ExecuteScript(
                "if (document.readyState) return document.readyState;"
            ).ToString();

            return readyState == expectedReadyState;
        }

        private static bool IsAutomationPaused(IWebDriver driver)
        {
            return driver.FindElements(By.CssSelector(".waiting, .tb-loading, [data-automation-paused]")).Count > 0;
        }

        #endregion Loading
        */
    }
}
