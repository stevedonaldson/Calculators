using Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

		[TestMethod]
		public void Test_Extrapolate()
		{
			var constituentData = _reader.ReadResource("Test.Resource.ConstituentExtrapolate.txt");
			var proxyData = _reader.ReadResource("Test.Resource.ProxyExtrapolate.txt");

			var result = Helpers.Extrapolate(constituentData.Array1, proxyData.Array1).Result;
			var sum = result.Sum();

			Assert.AreEqual(1827, result.Length);
			Assert.AreEqual(27.765595538846864144524838219M, result[0]);
			Assert.AreEqual(27.718878516777375852979152486M, result[1]);
			Assert.AreEqual(28.482582211576935684037153997M, result[23]);
			Assert.AreEqual(18.02M, result[1826]);
			Assert.AreEqual(37316.486132384076624111942895M, sum);
		}

		[TestMethod]
		public void Test_Benchmark()
		{
			var constituent1Data = _reader.ReadResource("Test.Resource.Constituent1.txt");
			var constituent2Data = _reader.ReadResource("Test.Resource.ConstituentExtrapolate.txt");
			var baseLineData = _reader.ReadResource("Test.Resource.ProxyExtrapolate.txt");
			var fxRateData = _reader.ReadResource("Test.Resource.FxRate.txt");

			var result = Helpers.Benchmark(constituent1Data.Array1, constituent2Data.Array1, baseLineData.Array1, fxRateData.Array1).Result;
			var sum1 = result.Bench1.Sum();
			var sum2 = result.Bench2.Sum();

			Assert.AreEqual(1827, result.Bench1.Length);
			Assert.AreEqual(1827, result.Bench2.Length);
			
			Assert.AreEqual(100M, result.Bench1[0]);
			Assert.AreEqual(102.01260179946884032507378346M, result.Bench1[1]);
			Assert.AreEqual(102.28569387148428544265930984M, result.Bench1[2]);
			Assert.AreEqual(60.613395803672772221463458580M, result.Bench1[1825]);
			Assert.AreEqual(58.083916952841340084499201600M, result.Bench1[1826]);
			Assert.AreEqual(128700.71733873922431572260641M, sum1);

			Assert.AreEqual(100M, result.Bench2[0]);
			Assert.AreEqual(99.65084634753953533442194635M, result.Bench2[1]);
			Assert.AreEqual(98.47090088893183655531110972M, result.Bench2[2]);
			Assert.AreEqual(87.17968565380527489515650473M, result.Bench2[1825]);
			Assert.AreEqual(82.99126372278804874673762891M, result.Bench2[1826]);
			Assert.AreEqual(155154.71821690021060872539273M, sum2);
		}

		[TestMethod]
		public void Test_DoMain()
		{
			var request = new BenchmarkRequest
			{
				Constituent1 = _reader.ReadResource("Test.Resource.Constituent1.txt").Array1,
				Constituent2 = _reader.ReadResource("Test.Resource.ConstituentExtrapolate.txt").Array1,
				IndexProxy = _reader.ReadResource("Test.Resource.ProxyExtrapolate.txt").Array1,
				FXRate = _reader.ReadResource("Test.Resource.FxRate.txt").Array1,
				Const1Weight = 0.6M,
				Const2Weight = 0.4M,
				AssetClassLeverageLongTermTargetLTV = 0.525M,
				AssetClassLeverageLTVImpliedInProxies = 0.5M
			};

			var result = Helpers.DoMain(request).Result;
		}
	}
}