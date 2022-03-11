using NUnit.Framework;

namespace CodeWars.Test.LeetCode
{
    public class ReverseInteger
    {
        [TestCase("2147483647", "0")]
        [TestCase("-2147483648", "0")]
        [TestCase("123", "321")]
        [TestCase("-123", "-321")]
        public void TestCases(int x, int expected)
        {
            var actual = Reverse(x);
            Assert.AreEqual(expected, actual);
            
            actual = ReverseInlineIntCheck(x);
            Assert.AreEqual(expected, actual);
        }

        public int ReverseInlineIntCheck(int x)
        {
            if (x < 10 && x > -10)
                return x;

            var lastResult = 0;
            var result = 0;
            while (x != 0)
            {
                lastResult = result;
                result = result * 10 + x % 10;
                x /= 10;
            }

            if (result / 10 != lastResult)
                    return 0;
            return result;
        }
        
        public int Reverse(int x)
        {
            if (x < 10 && x > -10)
                return x;
        
            long result = 0;
            while (x != 0)
            {
                result = result*10 + x%10;
                x /= 10;
            }
        
            if (result > int.MaxValue || result < int.MinValue)
                return 0;
            return (int)result;
        } 
    }
}