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
		private int[] Nums = Enumerable.Range(0, 1000000).ToArray();
		private decimal[] DecimalNums = Enumerable.Range(0, 1000000).ToArray().Select(y => Convert.ToDecimal(y)).ToArray();
		private double[] DoubleNums = Enumerable.Range(0, 1000000).ToArray().Select(y => Convert.ToDouble(y)).ToArray();

		[Benchmark(Baseline = true)]
		public void SumInt()
		{
			Helpers.CalculateSum(Nums);
		}

		[Benchmark]
		public void SumDecimal()
		{
			Helpers.CalculateSum(DecimalNums, 2);
		}
	}
}