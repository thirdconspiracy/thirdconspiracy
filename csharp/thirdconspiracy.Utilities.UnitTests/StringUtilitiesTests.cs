using System;
using System.Linq;
using NUnit.Framework;
using thirdconspiracy.Utilities.Extensions;

namespace thirdconspiracy.Utilities.Tests
{
    [TestFixture]
    public class StringUtilitiesTests
    {

        [Category("UnitTests"), Category("Utilities"), Category("Extensions")]
        [Test]
        [TestCase("", ',', StringSplitOptions.None)]
        [TestCase("a,b,c", ',', StringSplitOptions.None)]
        [TestCase("a,bb,ccc", ',', StringSplitOptions.None)]
        [TestCase("a,b,c,", ',', StringSplitOptions.None)]
        [TestCase("a,,b,c,d", ',', StringSplitOptions.None)]
        [TestCase(",a,b,cae,d,e,", ',', StringSplitOptions.None)]
        [TestCase("", ',', StringSplitOptions.RemoveEmptyEntries)]
        [TestCase("a,b,c", ',', StringSplitOptions.RemoveEmptyEntries)]
        [TestCase("a,bb,ccc", ',', StringSplitOptions.RemoveEmptyEntries)]
        [TestCase("a,b,c,", ',', StringSplitOptions.RemoveEmptyEntries)]
        [TestCase("a,,b,c,d", ',', StringSplitOptions.RemoveEmptyEntries)]
        [TestCase(",a,b,cae,d,e,", ',', StringSplitOptions.RemoveEmptyEntries)]
        public void SplitOptimized_Tests(string input, char separator, StringSplitOptions options)
        {
            var expected = input.Split(new[] { separator }, options);
            var actual = input.SplitOptimized(new[] { separator }, options).ToArray();

            Assert.AreEqual(expected, actual);
        }

    }
}