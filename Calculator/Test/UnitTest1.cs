using Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
	[TestClass]
	public class CalculatorTest
	{
		private ResourceReader _reader = null;

		public CalculatorTest()
		{
		}

		[TestInitialize]
		public void TestInitialize()
		{
			_reader = new ResourceReader();
		}


		[TestMethod]
		public void Test_Slope()
		{
			var data = _reader.ReadResource("Test.Resource.Slope.txt");
			var result = Helpers.CalculateSlope(data.Array1, data.Array2).Result;
			Assert.AreEqual(1.3739559993043003664561357483M, result);
		}

		[TestMethod]
		public void Test_Rendement()
		{
			var data = _reader.ReadResource("Test.Resource.Rendement.txt");
			var result = Helpers.CalculateRendement(data.Array1).Result;

			Assert.AreEqual(1826, result.Length);
			Assert.AreEqual(-0.0012247537180023582214446008M, result[0]);
			Assert.AreEqual(-0.0097338796898563529178789587M, result[1]);
			Assert.AreEqual(0.0126958280356598484679500048M, result[1824]);
			Assert.AreEqual(-0.0068795635845683385890653852M, result[1825]);
		}
	}
}