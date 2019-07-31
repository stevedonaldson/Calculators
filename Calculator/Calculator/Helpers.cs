using System;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator
{
	public struct BenchmarkData
	{
		public decimal[] Bench1 { get; set; }
		public decimal[] Bench2 { get; set; }
	}

	public static class Helpers
	{
		//public static long CalculateSum(int[] nums)
		//{
		//	long total = 0;

		//	Parallel.For<long>(
		//		0,
		//		nums.Length,
		//		() => 0,
		//		(index, state, subTotal) =>
		//		{
		//			subTotal += nums[index];
		//			return subTotal;
		//		},
		//		(subTotal) => Interlocked.Add(ref total, subTotal));

		//	return total;
		//}

		//public static Task<long> CalculateSumAsync(int[] nums)
		//{
		//	return Task.Run(() => CalculateSum(nums));
		//}

		//public static decimal CalculateSum(decimal[] nums)
		//{
		//	long total = 0;
		//	decimal multiplier = 100000000M;

		//	//for (int i = 0; i < decimalPlaces; i++)
		//	//{
		//	//	multiplier *= 10;
		//	//}

		//	Parallel.For<long>(
		//		0, 
		//		nums.Length, 
		//		() => 0, 
		//		(index, state, subTotal) =>
		//		{
		//			subTotal += decimal.ToInt64(nums[index] * multiplier);
		//			return subTotal;
		//		},
		//		(subTotal) => Interlocked.Add(ref total, subTotal));

		//	return total / multiplier;
		//}

		public static decimal CalculateSum(decimal[] nums)
		{
			decimal total = 0;

			for (int i = 0; i < nums.Length; i++)
			{
				total += nums[i];
			}

			return total;
		}

		public static Task<decimal> CalculateSumAsync(decimal[] nums)
		{
			return Task.Run(() => CalculateSum(nums));
		}

		public static decimal CalculateAverge(decimal[] nums)
		{
			var sum = CalculateSum(nums);
			return sum / nums.Length;
		}

		public static async Task<decimal> CalculateAvergeAsync(decimal[] nums)
		{
			var sum = await CalculateSumAsync(nums);
			return sum / nums.Length;
		}

		public static TOutput[] CalculateWithFrame<TInput, TOutput>(TInput[] input, Action<int, TInput[], TOutput[]> action)
		{
			TOutput[] output = new TOutput[input.Length];
			Parallel.For(0, input.Length, (index, state) => action(index, input, output));
			return output;
		}

		public static TOutput[] CalculateWithFrameInit<TInput, TOutput>(TInput[] input, Func<TInput[], TOutput[]> init, Action<int, TInput[], TOutput[]> action)
		{
			TOutput[] output = init(input);
			Parallel.For(0, input.Length, (index, state) => action(index, input, output));
			return output;
		}

		public static Task<TOutput[]> CalculateWithFrameAsync<TInput, TOutput>(TInput[] input, Action<int, TInput[], TOutput[]> action)
		{
			return Task.Run(() => CalculateWithFrame(input, action));
		}

		public static Task<TOutput[]> CalculateWithFrameInitAsync<TInput, TOutput>(TInput[] input, Func<TInput[], TOutput[]> init, Action<int, TInput[], TOutput[]> action)
		{
			return Task.Run(() => CalculateWithFrameInit(input, init, action));
		}

		public static async Task<decimal> CalculateSlope(decimal[] x, decimal[] y)
		{
			var xAveTask = CalculateAvergeAsync(x);
			var yAveTask = CalculateAvergeAsync(y);

			var xAve = await xAveTask;

			var xMinusXAveTask = CalculateWithFrameAsync<decimal, decimal>(x, (index, input, output) => output[index] = input[index] - xAve);

			var yAve = await yAveTask;

			var yMinusYAveTask = CalculateWithFrameAsync<decimal, decimal>(y, (index, input, output) => output[index] = input[index] - yAve);

			var xMinusXAve = await xMinusXAveTask;

			var xMinusXAveSquaredTask = CalculateWithFrameAsync<decimal, decimal>(xMinusXAve, (index, input, output) => output[index] = input[index] * input[index]);

			var yMinusYAve = await yMinusYAveTask;

			var xMinusXAveMultiplyYMinusYAveTask = CalculateWithFrameAsync<decimal, decimal>(xMinusXAve, (index, input, output) => output[index] = input[index] * yMinusYAve[index]);

			var xMinusXAveSquared = await xMinusXAveSquaredTask;

			var denominatorTask = CalculateSumAsync(xMinusXAveSquared);

			var xMinusXAveMultiplyYMinusYAve = await xMinusXAveMultiplyYMinusYAveTask;

			var numeratorTask = CalculateSumAsync(xMinusXAveMultiplyYMinusYAve);

			var denominator = await denominatorTask;
			var numerator = await numeratorTask;

			return numerator / denominator;
		}
		public static decimal CalculateSlopeNoAsync(decimal[] x, decimal[] y)
		{
			var xAve = CalculateAverge(x);
			var yAve = CalculateAverge(y);

			var xMinusXAve = CalculateWithFrame<decimal, decimal>(x, (index, input, output) => output[index] = input[index] - xAve);
			var yMinusYAve = CalculateWithFrame<decimal, decimal>(y, (index, input, output) => output[index] = input[index] - yAve);

			var xMinusXAveSquared = CalculateWithFrame<decimal, decimal>(xMinusXAve, (index, input, output) => output[index] = input[index] * input[index]);
			var xMinusXAveMultiplyYMinusYAve = CalculateWithFrame<decimal, decimal>(xMinusXAve, (index, input, output) => output[index] = input[index] * yMinusYAve[index]);

			var denominator = CalculateSum(xMinusXAveSquared);
			var numerator = CalculateSum(xMinusXAveMultiplyYMinusYAve);

			return numerator / denominator;
		}

		public static Task<decimal[]> CalculateRendement(decimal[] x)
		{
			return CalculateWithFrameInitAsync(
				x,
				(input) => new decimal[input.Length - 1],
				(index, input, output) =>
				{
					if (index != 0)
						output[index - 1] = input[index] / input[index - 1] - 1;
				});
		}
		public static decimal[] CalculateRendementNoAsync(decimal[] x)
		{
			return CalculateWithFrameInit(
				x,
				(input) => new decimal[input.Length - 1],
				(index, input, output) =>
				{
					if (index != 0)
						output[index - 1] = input[index] / input[index - 1] - 1;
				});
		}

		public static async Task<decimal[]> Extrapolate(decimal[] lessData, decimal[] baseLine)
		{
			var gapLength = baseLine.Length - lessData.Length;

			if (gapLength < 0)
				throw new Exception("Baseline length cannot be less then length of LessData");

			// No extrapolation needed
			if (gapLength == 0)
				return lessData;

			// Begin Extrapolation
			var lessDataTask = CalculateRendement(lessData);
			var baseLineTask = CalculateRendement(baseLine);

			var lessDataIncrements = await lessDataTask;
			var baseLineIncrements = await baseLineTask;

			var baseLineIncrementsSection = new Span<decimal>(baseLineIncrements, gapLength, baseLineIncrements.Length - gapLength).ToArray();
			var slope = await CalculateSlope(baseLineIncrementsSection, lessDataIncrements);

			var gaps = new decimal[gapLength];
			for (int i = 0; i < gapLength; i++)
				gaps[i] = baseLineIncrements[i] * slope;

			var finalResult = new decimal[baseLine.Length];
			Array.Copy(lessData, 0, finalResult, gapLength, baseLine.Length - gapLength);
			
			for (int i = gaps.Length - 1; i > -1; i--)
				finalResult[i] = finalResult[i + 1] / (1 + gaps[i]);

			return finalResult;
		}

		//public static decimal[] ExtrapolateNoAsync(decimal[] lessData, decimal[] baseLine)
		//{
		//	var gapLength = allData.Length - lessData.Length;

		//	var constituentIncrements = CalculateRendementNoAsync(lessData);
		//	var proxyIncrements = CalculateRendementNoAsync(allData);

		//	var proxyIncrementsSection = new Span<decimal>(proxyIncrements, gapLength, proxyIncrements.Length - gapLength).ToArray();
		//	var slope = CalculateSlopeNoAsync(proxyIncrementsSection, constituentIncrements);

		//	var constituentIncrementsGaps = new decimal[gapLength];
		//	for (int i = 0; i < gapLength; i++)
		//		constituentIncrementsGaps[i] = proxyIncrements[i] * slope;

		//	var finalResult = new decimal[allData.Length];
		//	Array.Copy(lessData, 0, finalResult, gapLength, allData.Length - gapLength);

		//	for (int i = constituentIncrementsGaps.Length - 1; i > -1; i--)
		//		finalResult[i] = finalResult[i + 1] / (1 + constituentIncrementsGaps[i]);

		//	return finalResult;
		//}

		public static async Task<BenchmarkData> Benchmark(decimal[] constituent1, decimal[] constituent2, decimal[] baseline, decimal[] exchangeRate)
		{
			Task<decimal[]> extrapolateTask = null;
			decimal[] newConstituent2 = null;

			var constituent1IncrementTask = CalculateRendement(constituent1);

			if (constituent2.Length != baseline.Length)
				extrapolateTask = Extrapolate(constituent2, baseline);
			else
				newConstituent2 = constituent2;

			var constituent1Increment = await constituent1IncrementTask;
			decimal adjustmentLevel = 1.052631579M;

			var constituent1AdjustmentTask = CalculateWithFrameAsync<decimal, decimal>(constituent1Increment, (i, input, output) => output[i] = input[i] * adjustmentLevel);

			if (extrapolateTask != null)
				newConstituent2 = await extrapolateTask;

			var constituent2WithXRateTask = CalculateWithFrameAsync<decimal, decimal>(newConstituent2, (i, input, output) => output[i] = input[i] * exchangeRate[i]);
			var constituent2WithXRate = await constituent2WithXRateTask;

			var constituent2IncrementTask = CalculateRendement(constituent2WithXRate);
			var constituent2Increment = await constituent2IncrementTask;

			var constituent2AdjustmentTask = CalculateWithFrameAsync<decimal, decimal>(constituent2Increment, (i, input, output) => output[i] = input[i] * adjustmentLevel);

			var constituent1Adjustment = await constituent1AdjustmentTask;
			var constituent1BenchTask = GetBench(constituent1Adjustment);

			var constituent2Adjustment = await constituent2AdjustmentTask;
			var constituent2BenchTask = GetBench(constituent2Adjustment);

			return new BenchmarkData
			{
				Bench1 = await constituent1BenchTask,
				Bench2 = await constituent2BenchTask
			};
		}

		private static async Task<decimal[]> GetBench(decimal[] adjustment)
		{
			return await Task.Run(() =>
			{
				var bench = new decimal[adjustment.Length + 1];
				bench[0] = 100M;

				for (int i = 1; i < bench.Length; i++)
				{
					bench[i] = bench[i - 1] * (1 + adjustment[i - 1]);
				}

				return bench;
			});
		}
	}
}