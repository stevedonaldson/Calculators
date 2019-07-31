using BenchmarkDotNet.Running;

namespace Bench
{
	class Program
	{
		static void Main()
		{
			_ = BenchmarkRunner.Run<Benchmarks>();
		}
	}
}