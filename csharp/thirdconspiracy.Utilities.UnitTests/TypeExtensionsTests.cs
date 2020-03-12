using NUnit.Framework;
using thirdconspiracy.Utilities.Extensions;

namespace thirdconspiracy.Utilities.UnitTests
{
	[TestFixture]
	public class TypeExtensionsTests
	{

		[Category("UnitTests"), Category("Extensions")]
		[TestCase("15697000001111", true)]
		[TestCase("357805023984942", true)]
		[TestCase("357805023984943", false)]
		[TestCase("fake05023984942", false)]
		public void ImeiTypeTests(string imei, bool expected)
		{
			var actual = imei.IsImeiType();
			Assert.AreEqual(expected, actual);
		}

		[Category("UnitTests"), Category("Extensions")]
		[TestCase("9780439064866", true)]
		[TestCase("9780439064867", false)]
		[TestCase("fake439064867", false)]
		public void GtinTypeTests(string gtin, bool expected)
		{
			var actual = gtin.IsGtinType();
			Assert.AreEqual(expected, actual);
		}

		//TODO: ISSN & ISBN

	}
}