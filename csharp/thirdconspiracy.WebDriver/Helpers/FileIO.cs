using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.WebDriver.Core
{
    public static class FileIO
    {

        /*
        /// <summary>
        /// Download a file via C# directly using cookies from current browser.  We assume we know the URL
        /// of the file being downloaded
        /// </summary>
        /// <param name="driver">Your current WebDriver driver</param>
        /// <param name="webPath">URL of the file</param>
        /// <param name="localPath">Name and location to save file</param>
        public static void DownloadFile(IWebDriver driver, string webPath, string localPath)
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Cookie] = cookieString(driver);
            client.DownloadFile(webPath, localPath);
        }

        private static string cookieString(IWebDriver driver)
        {
            var cookies = driver.Manage().Cookies.AllCookies;
            //return string.Join("; ", cookies.Select(c => string.Format("{0}={1}", c.Name, c.Value)));
            string cookiestring = "";
            foreach (var cookie in cookies)
            {
                cookiestring += string.Format("{0}={1}; ", cookie.Name, cookie.Value);
            }
            return cookiestring;
        }
        */

    }
}
