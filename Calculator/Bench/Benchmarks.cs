using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Calculator;
using System;
using System.Linq;

namespace Bench
{
	[RankColumn]
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	public class Benchmarks
	{
		//private int[] Nums = Enumerable.Range(0, 1000000).ToArray();
		//private decimal[] DecimalNums = Enumerable.Range(0, 1000000).ToArray().Select(y => Convert.ToDecimal(y)).ToArray();
		//private double[] DoubleNums = Enumerable.Range(0, 1000000).ToArray().Select(y => Convert.ToDouble(y)).ToArray();

		private readonly decimal[] LessData;
		private readonly decimal[] AllData;

		public Benchmarks()
		{
			var reader = new ResourceReader();
			var constituentData = reader.ReadResource("Bench.Resource.ConstituentExtrapolate.txt");

			var reader2 = new ResourceReader();
			var proxyData = reader2.ReadResource("Bench.Resource.ProxyExtrapolate.txt");

			LessData = constituentData.Array1;
			AllData = proxyData.Array1;
		}

		[Benchmark(Baseline = true)]
		public void Extrapolate()
		{
			_ = Helpers.Extrapolate(LessData, AllData).Result;
		}

		[Benchmark]
		public void ExtrapolateNoAsync()
		{
			//_ = Helpers.ExtrapolateNoAsync(LessData, AllData);
		}
	}
}