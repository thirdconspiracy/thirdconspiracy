using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OpenQA.Selenium.Support.Events;
using thirdconspiracy.WebDriver.Core;

namespace thirdconspiracy.WebDriver.Driver
{
    public static class DriverSessions
    {

        private static Dictionary<string, object> _assembly = new Dictionary<string, object>();
        public static Assembly GetAssembly(string type)
        {
            return _assembly[type] as Assembly;
        }

        public static void AddAssembly(string myType, Assembly assembly)
        {
            bool containsKey = _assembly.ContainsKey(myType);
            if (!containsKey)
            {
                _assembly.Add(myType, assembly);
            }
        }

    }
}
