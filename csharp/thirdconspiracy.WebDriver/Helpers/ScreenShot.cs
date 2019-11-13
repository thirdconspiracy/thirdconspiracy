using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;

namespace thirdconspiracy.WebDriver.Helpers
{
    public static class ScreenShot
    {
        private static readonly string ImagePath = ".";

        #region DriverEvents

        public static void DriverExceptionEvent(object sender, WebDriverExceptionEventArgs e)
        {
            if (e.Driver == null)
            {
                return;
            }

            SaveImage(e.Driver);
            SaveSource(e.Driver);
        }

        #endregion DriverEvents

        #region HTML Source Code

        public static void SaveSource(this IWebDriver driver)
        {
            Console.WriteLine("Saving Driver Source");

            var path = GetSaveLocation();
            var cleanFilename = GetTestName(TestContext.CurrentContext.Test.Name, "");
            var filename = $"{path}\\{cleanFilename}.html";
            File.WriteAllText(filename, driver.PageSource);
        }

        #endregion HTML Source Code

        #region Image

        public static void SaveImage(this IWebDriver driver)
        {
            Console.WriteLine("Saving Driver Image");

            var path = GetSaveLocation();
            var cleanFilename = GetTestName(TestContext.CurrentContext.Test.Name, "");
            var filename = $"{path}\\{cleanFilename}.jpeg";
            WriteImage(driver, filename);
        }

        /// <summary>
        /// Saves a screen shot of current active screen
        /// </summary>
        /// <param name="driver">The current browser you're using</param>
        /// <param name="filename">filename=("path" + "name" + ".jpg")</param>
        public static void WriteImage(IWebDriver driver, string filename)
        {
            var screenshotDriver = driver as ITakesScreenshot;
            var screenshot = screenshotDriver.GetScreenshot();
            //Save as a jpg file
            screenshot.SaveAsFile(filename, ScreenshotImageFormat.Jpeg);
            Console.WriteLine("    -> Image saved to '{0}'", filename);
        }


        #endregion Image

        #region Helpers

        public static string GetSaveLocation()
        {
            //TODO: fix this
            return $"{ImagePath}\\nunitMetadata";
        }

        private static string GetTestName(string fileName, string replacementCharacterToUse)
        {
            //Remove Invalid Chars
            var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var regex = new Regex($"[{Regex.Escape(invalidChars)}]");
            var cleanFilename = regex.Replace(fileName, replacementCharacterToUse);

            //Replace whitespace
            regex = new Regex(@"\s+");
            cleanFilename = regex.Replace(cleanFilename, "_");
            return cleanFilename;
        }

        #endregion Helpers

    }
}
