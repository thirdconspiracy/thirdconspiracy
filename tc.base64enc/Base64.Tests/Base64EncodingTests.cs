using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using tc.Base64;

namespace Base64.Tests
{
    [TestFixture]
    public class Base64EncodingTests
    {

        [TestCase("45766964696e74", "RXZpZGludA==")]
        [TestCase("ABCD", "q80=")]
        public void TestConversion(string input, string expectedResult)
        {
            var encodedResult = Encode.HexToBase64(input);
            Assert.AreEqual(expectedResult, encodedResult);
        }

    }
}
