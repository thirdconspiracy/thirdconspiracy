using NUnit.Framework;

namespace CodeWars.Test.LeetCode
{
    public class LongestPalindromeSubstring
    {
        [TestCase("babbad", "abba")]
        [TestCase("babzbad", "abzba")]
        [TestCase("babad", "bab")]
        [TestCase("cbbd", "bb")]
        [TestCase("tattarrattat", "tattarrattat")]
        public void TestCases(string input, string expected)
        {
            var actual = LongestPalindrome(input);
            Assert.AreEqual(expected, actual);
        }

        private static string LongestPalindrome(string s)
        {
            if (s.Length < 2)
                return s;
            var max = 1;
            var maxIndex = 0;

            for (var i = 1; i < s.Length; i++)
            {
                if (IsPalindrome(s, i, max)) //Odd
                {
                    max++;
                    maxIndex = i - max + 1;
                }
                else if (IsPalindrome(s, i, max + 1)) //Even
                {
                    max += 2;
                    maxIndex = i - max + 1;
                }
            }

            return s.Substring(maxIndex, max);
        }

        private static bool IsPalindrome(string s, int i, int len)
        {
            if (i - len < 0)
                return false;
            

            var l = i - len;
            var left = s[l];
            var right = s[i];
            
            if (left != right)
                return false;
            
            var iterations = (len + 1) / 2;
            for (var j = 1; j < iterations; j++)
            {
                left = s[l + j];
                right = s[i - j];
                if (left != right)
                    return false;
            }

            return true;
        }
    }
}