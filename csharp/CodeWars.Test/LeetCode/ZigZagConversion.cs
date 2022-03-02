using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode
{
    public class ZigZagConversion
    {
        [TestCase("PAYPALISHIRING", 1, "PAYPALISHIRING")]
        [TestCase("PAYPALISHIRING", 3, "PAHNAPLSIIGYIR")]
        [TestCase("PAYPALISHIRING", 4, "PINALSIGYAHRPI")]
        public void TestCases(string input, int numRows, string expectedResult)
        {
            var actual = Convert(input, numRows);
            Assert.AreEqual(expectedResult, actual);
        }

        public string Convert(string s, int numRows)
        {
            //return ConvertLinear(s, numRows);
            return ConvertCyclic(s, numRows);
        }

        private string ConvertCyclic(string s, int numRows)
        {
            if (numRows == 1 || s.Length < 3 || s.Length <= numRows)
                return s;

            var length = numRows + numRows - 2;
            var sb = new StringBuilder();
            for (var row = 0; row < numRows; ++row)
            {
                for (var cycle = 0; cycle * length < s.Length; ++cycle)
                {
                    var index = cycle * length + row;
                    if (index >= s.Length)
                        continue;
                    sb.Append(s[index]);

                    if (row == 0 || row == numRows - 1)
                        continue;

                    var zipIndex = (cycle + 1) * length - row;
                    if (zipIndex < s.Length)
                        sb.Append(s[zipIndex]);
                }
            }

            return sb.ToString();
        }

        private string TraverseStringCyclically_v3(string s, int numRows)
        {
            if (numRows == 1) return s;
            StringBuilder sb = new StringBuilder();
            int length = 2 * numRows - 2;
            for (int row = 0; row < numRows; ++row)
            {
                for (int cycle = 0; cycle * length < s.Length; ++cycle)
                {
                    var index = cycle * length + row;
                    if (row == 0)
                    {
                        sb.Append(s[index]);
                        continue;
                    }

                    if (index < s.Length)
                        sb.Append(s[index]);

                    if (row != numRows - 1 && (cycle + 1) * length - row < s.Length)
                        sb.Append(s[(cycle + 1) * length - row]);
                }
            }

            return sb.ToString();
        }

        private static string ConvertLinear(string s, int numRows)
        {
            if (numRows == 1 || s.Length < 3)
                return s;

            var sbs = new List<StringBuilder>();
            for (var i = 0; i < numRows; i++)
                sbs.Add(new StringBuilder());

            var direction = 1;
            var current = 0;
            for (var i = 0; i < s.Length; i++)
            {
                sbs[current].Append(s[i]);
                current += direction;

                if (current == 0)
                    direction = 1;
                else if (current == numRows - 1)
                    direction = -1;
            }

            for (var i = 1; i < numRows; i++)
            {
                sbs[0].Append(sbs[i].ToString());
            }

            return sbs[0].ToString();
        }
    }
}