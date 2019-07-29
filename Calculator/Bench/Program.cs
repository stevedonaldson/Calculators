using BenchmarkDotNet.Running;

namespace Bench
{
	class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<Benchmarks>();

			//var nums = Enumerable.Range(0, 1000000).ToArray();
			//var decimalNums = nums.Select(y => Convert.ToDecimal(y)).ToArray();
			//var result = Helpers.CalculateSum(decimalNums, 2);



			//var nums2 = Enumerable.Range(0, 1000000).ToArray();
			//var result2 = Helpers.CalculateSum(nums2);





		}
	}
}