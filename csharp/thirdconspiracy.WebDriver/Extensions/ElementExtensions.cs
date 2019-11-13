using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using thirdconspiracy.WebDriver.Constants;

namespace thirdconspiracy.WebDriver.Extensions
{
    public static class ElementExtensions
    {

        /*
        /// <summary>
        /// Requires finding element by FindElementSafe(By).
        /// Returns T/F depending on if element is defined or null.
        /// </summary>
        /// <author>Brantley Blanchard</author>
        /// <param name="element">Current element</param>
        /// <returns>Returns T/F depending on if element is defined or null.</returns>
        public static bool Exists(this IWebElement element)
        {
            return element != null;
        }

        #region Select Element

        public static string SelectElementText(this IWebElement element)
        {
            try
            {
                return new SelectElement(element).SelectedOption.Text;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion Select Element

        #region jquery

        public static string GetInnerHtml(this IWebElement element, IWebDriver driver)
        {
            var jse = driver as IJavaScriptExecutor;
            var innerHtml = jse.ExecuteScript("return arguments[0].innerHTML;", element).ToString();
            return innerHtml;
        }

        public static bool Checked(this IWebElement element)
        {
            return element.Selected;
        }
        public static void Checked(this IWebElement element, bool select)
        {
            if (!element.Selected.Equals(select)) { element.Click(); }
        }

        /// <summary>
        /// Returns the "value" set for that element
        /// </summary>
        /// <param name="element">the element calling this method</param>
        /// <returns></returns>
        public static string val(this IWebElement element)
        {
            return element.Value();
        }

        public static string Value(this IWebElement element)
        {
            return element.GetAttribute("value");
        }

        #endregion jquery

        */
    }
}
