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
		[TestCase("000000000000000", false)]
		public void ImeiTypeTests(string imei, bool expected)
		{
			var actual = imei.IsImeiType();
			Assert.AreEqual(expected, actual);
		}

		[TestCase("8965880812100011146", true)]
		[TestCase("8965880812100011147", false)]
		[TestCase("fake880812100011146", false)]
		public void IccidTypeTests(string iccid, bool expected)
		{
			var actual = iccid.IsIccidType();
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