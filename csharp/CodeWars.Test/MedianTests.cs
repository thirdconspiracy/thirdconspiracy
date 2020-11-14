using System;
using NUnit.Framework;
using thirdconspiracy.Math;

namespace CodeWars.Test
{
    public class MedianTests
    {
        [Test]
        public void TestRunningMedian()
        {
            var expectedResult = Expected2();
            var verify = expectedResult.Split(" ");
            var numbers = GetNumbers2();
            var runningMedian = Median.RunningMedian(numbers, verify);
            var actualResult = string.Join(" ", runningMedian);
            Assert.AreEqual(expectedResult, actualResult);
        }

        private static int[] GetNumbers2()
        {
            var numbers = new [] {94455, 20555, 20535, 53125, 73634, 148, 63772, 17738, 62995, 13401, 95912, 13449, 92211, 17073, 69230,
                22016, 22120, 78563, 16571, 1817, 41510, 74518, 81638, 89659, 60445, 35597, 15237, 88830, 26019, 77519, 10914, 36827,
                98074, 31450, 89952, 71709, 31598, 70076, 5799, 10945 };
            return numbers;
        }
        private static string Expected2()
        {
            var expectedResult = "94455.0 57505.0 20555.0 36840.0 53125.0 36840.0 53125.0 36840.0 53125.0 36840.0 53125.0 36840.0 53125.0 "+
            "36840.0 53125.0 37570.5 22120.0 37622.5 22120.0 22068.0 22120.0 31815.0 41510.0 47317.5 53125.0 47317.5 41510.0 47317.5 41510.0 "+
            "47317.5 41510.0 39168.5 41510.0 39168.5 41510.0 47317.5 41510.0 47317.5 41510.0 39168.5";
            return expectedResult;
        }

        private static string Expected1()
        {
            var expectedResult = "1.0 1.5 2.0 2.5 3.0 3.5 4.0 4.5 5.0 5.5";
            return expectedResult;
        }

        private static int[] GetNumbers1()
        {
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            return numbers;
        }
    }
}
