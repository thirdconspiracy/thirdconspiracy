using System;
using System.Linq;
using NUnit.Framework;
using thirdconspiracy.Utilities.Extensions;

namespace thirdconspiracy.Utilities.Tests
{
	[TestFixture]
	public class StringExtensionsTests
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

		[Category("UnitTests"), Category("Extensions")]
		[TestCase("THEQuickBrownFox", "THE Quick Brown Fox")]
		[TestCase("theQUICKBrownFox", "The QUICK Brown Fox")]
		[TestCase("TheQuickBrownFOX", "The Quick Brown FOX")]
		[TestCase("TheQuickBrownFox", "The Quick Brown Fox")]
		[TestCase("the_quick_brown_fox", "The Quick Brown Fox")]
		[TestCase("theFOX", "The FOX")]
		[TestCase("FOX", "FOX")]
		[TestCase("QUICK", "QUICK")]
		public void TitleCaseTests(string input, string expected)
		{
			var actual = input.ToTitleCase();
			Assert.AreEqual(expected, actual);

		}


		[Category("UnitTests"), Category("Extensions")]
		[TestCase("abcdef", 3, "def")]
		[TestCase("abcdef", 6, "abcdef")]
		[TestCase("abcdef", 30, "abcdef")]
		[TestCase("", 30, "")]
		[TestCase(null, 30, "")]
		public void TakeRightTests(string text, int length, string expected)
		{
			var actual = text.TakeRight(length);
			Assert.AreEqual(expected, actual);
		}

		[Category("UnitTests"), Category("Extensions")]
		[TestCase("true", true)]
		[TestCase("True", true)]
		[TestCase("TRUE", true)]
		[TestCase("t", true)]
		[TestCase("T", true)]
		[TestCase("1", true)]
		[TestCase("On", true)]
		[TestCase("enabled", true)]
		[TestCase("False", false)]
		[TestCase("F", false)]
		[TestCase("0", false)]
		[TestCase("Off", false)]
		[TestCase("disabled", false)]
		public void TryParseBool(string input, bool expected)
		{
			input.TryParseBool(out var actual);
			Assert.AreEqual(expected, actual);
		}

	}
}