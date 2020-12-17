using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CodeWars.Test
{
    public class JesseAndCookiesTests
    {
        //[TestCase(1, "1 1 1", -1)]
        [TestCase(6, "1 2 3 9 10 12", 2)]
        public void Test1(int k, string cookies, int expectedResult)
        {
            var a = cookies
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(n => int.Parse(n))
                .ToArray();
            var i = Cookies(k, a);
            Assert.AreEqual(expectedResult, i);
        }

        static int Cookies(int k, int[] arr)
        {
            Array.Sort(arr);
            var fullPtr = 0;
            var sweetPtr = 0;

            var fullLength = arr.Length;
            var sweetLength = 0;

            var operations = 0;

            try
            {
                while (arr[fullPtr] < k || arr[sweetPtr] < k)
                {
                    var tmpCookie1 = sweetPtr == sweetLength || arr[fullPtr] <= arr[sweetPtr]
                        ? arr[fullPtr++]
                        : arr[sweetPtr++];
                    var tmpCookie2 = sweetPtr == sweetLength || (fullPtr != fullLength && arr[fullPtr] <= arr[sweetPtr])
                        ? arr[fullPtr++]
                        : arr[sweetPtr++];
                    arr[sweetLength++] = tmpCookie1 + 2 * tmpCookie2;
                    operations++;

                    //End of array
                    if (fullPtr == fullLength)
                    {
                        //new end = last sweetened
                        fullLength = sweetLength;
                        //clear sweet collection so we pull from full
                        sweetLength = sweetPtr;
                        //start at last sweet location
                        fullPtr = sweetPtr;
                    }
                }

                return operations;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
