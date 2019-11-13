using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.WebDriver.Constants
{
    public static class WebDriverConstants
    {

        public static class Timeout
        {
            public static readonly int DefaultTimeout = 240;
            public static readonly int CdnDefaultTimeout = 120;
            public static readonly int ImmediateTimeout = 0;
            public static readonly int Sixty = 60;
            public static readonly int Thirty = 30;
        }

        public static class LocalPath
        {
            public static readonly string Driver = "todo";
            public static readonly string SeleniumHub = "todo";
        }

    }
}
