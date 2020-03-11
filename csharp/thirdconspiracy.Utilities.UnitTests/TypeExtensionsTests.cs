using System;
using System.Linq;
using NUnit.Framework;
using thirdconspiracy.Utilities.Extensions;

namespace thirdconspiracy.Utilities.Tests
{
	[TestFixture]
	public class TypeExtensionsTests
	{

		[Category("UnitTests"), Category("Extensions")]
		[Test]
		[TestCase("15697000001111", true)]
		[TestCase("357805023984942", true)]
		[TestCase("357805023984943", false)]
		[TestCase("fake05023984942", false)]
		public void ImeiTypeTests(string imei, bool expected)
		{
			var actual = imei.IsImeiType();
			Assert.AreEqual(expected, actual);
		}

	}
}