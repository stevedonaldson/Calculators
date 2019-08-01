using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator
{
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

		public static async Task<decimal[]> ExtrapolateNew(decimal[] lessData, decimal[] baseLine)
		{
			var gapLength = baseLine.Length - lessData.Length;

			if (gapLength < 0)
				throw new Exception("Baseline length cannot be less then length of LessData");

			// No extrapolation needed
			if (gapLength == 0)
				return lessData;

			var baseLineIncrementsSection = new Span<decimal>(baseLine, gapLength, baseLine.Length - gapLength).ToArray();
			var slope = await CalculateSlope(baseLineIncrementsSection, lessData);

			var gaps = new decimal[gapLength];
			for (int i = 0; i < gapLength; i++)
				gaps[i] = baseLine[i] * slope;

			return gaps;
		}

		public static async Task<BenchmarkResponse> Benchmark(decimal[] constituent1, decimal[] constituent2, decimal[] baseline, decimal[] exchangeRate)
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

			return new BenchmarkResponse
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

		//------------------------------------------------------------------------------------------------------------------------------------------------

		public static async Task<BenchmarkResponse> DoMain(BenchmarkRequest request)
		{
			var taskDictionary = new Dictionary<string, object>();

			var task0 = CalculateAdjustmentLevel(new AdjustmentLevelRequest
			{
				AssetClassLeverageLongTermTargetLTV = request.AssetClassLeverageLongTermTargetLTV,
				AssetClassLeverageLTVImpliedInProxies = request.AssetClassLeverageLTVImpliedInProxies
			});
			taskDictionary.Add("AdjustmentLevel", task0);

			var task1 = CalculateIndexIncrements(new IndexIncrementsRequest
			{
				Data = request.IndexProxy
			});
			taskDictionary.Add("IndexIncrements", task1);

			var task2 = BenchmarkExtrapolateHistory(new BenchmarkExtrapolateHistoryRequest
			{
				Tasks = taskDictionary,
				Constituent = request.Constituent2,
				Index = request.IndexProxy
			});
			taskDictionary.Add("BenchmarkExtrapolateHistory", task2);

			var task3 = CalculateLocalBenchmark(new LocalBenchmarkRequest
			{
				Tasks = taskDictionary,
				Constituent1 = request.Constituent1,
				Const1Weight = request.Const1Weight,
				Const2Weight = request.Const2Weight
			});
			taskDictionary.Add("LocalBenchmark", task3);

			var task4 = CalculateBenchmarkLevel(new BenchmarkLevelRequest
			{
				Tasks = taskDictionary,
				Constituent1 = request.Constituent1,
				FxRate = request.FXRate
			});
			taskDictionary.Add("BenchmarkLevel", task4);



			var response3 = await task3;
			var response4 = await task4;

			return new BenchmarkResponse
			{
				Bench1 = response4.Constituent1,
				Bench2 = response4.Constituent2
			};
		}

		private static async Task<IndexIncrementsResponse> CalculateIndexIncrements(IndexIncrementsRequest request)
		{
			var response = new IndexIncrementsResponse
			{
				PercentIncrements = await CalculateRendement(request.Data)
			};

			return response;
		}

		private static Task<AdjustmentLevelResponse> CalculateAdjustmentLevel(AdjustmentLevelRequest request)
		{
			return Task.FromResult(new AdjustmentLevelResponse
			{
				AdjustmentLevel = (1 - request.AssetClassLeverageLTVImpliedInProxies) / (1 - request.AssetClassLeverageLongTermTargetLTV)
			});
		}

		private static async Task<BenchmarkExtrapolateHistoryResponse> BenchmarkExtrapolateHistory(BenchmarkExtrapolateHistoryRequest request)
		{
			var constIncrementTask = CalculateRendement(request.Constituent);
			var indexIncrement = (await (request.Tasks["IndexIncrements"] as Task<IndexIncrementsResponse>)).PercentIncrements;
			var constIncrement = await constIncrementTask;

			var extrapolated = await ExtrapolateNew(constIncrement, indexIncrement);

			var finalResult = new decimal[request.Index.Length];
			Array.Copy(request.Constituent, 0, finalResult, extrapolated.Length, finalResult.Length - extrapolated.Length);

			for (int i = extrapolated.Length - 1; i > -1; i--)
				finalResult[i] = finalResult[i + 1] / (1 + extrapolated[i]);

			return new BenchmarkExtrapolateHistoryResponse
			{
				Constituent = finalResult
			};
		}

		private static async Task<LocalBenchmarkResponse> CalculateLocalBenchmark(LocalBenchmarkRequest request)
		{
			var constituent1 = request.Constituent1;
			var constituent2 = (await (request.Tasks["BenchmarkExtrapolateHistory"] as Task<BenchmarkExtrapolateHistoryResponse>)).Constituent;
			var const1Weight = request.Const1Weight;
			var const2Weight = request.Const2Weight;

			var response = new LocalBenchmarkResponse
			{
				Data = await CalculateWithFrameInitAsync(
					constituent1,
					(input) => new decimal[input.Length - 1],
					(index, input, output) =>
					{
						if (index != 0)
							output[index - 1] = (input[index] / input[index - 1] - 1) * const1Weight + (constituent2[index] / constituent2[index - 1] - 1) * const2Weight;
					})
			};

			return response;
		}

		private static async Task<BenchmarkLevelResponse> CalculateBenchmarkLevel(BenchmarkLevelRequest request)
		{
			var constituent1 = request.Constituent1;
			var constituent2 = (await (request.Tasks["BenchmarkExtrapolateHistory"] as Task<BenchmarkExtrapolateHistoryResponse>)).Constituent;
			var fxRate = request.FxRate;

			var const1IncrementTask = CalculateRendement(constituent1);
			var const2Converted = await CalculateWithFrameAsync<decimal, decimal>(constituent2, (i, input, output) => output[i] = input[i] * fxRate[i]);

			var const2Increment = await CalculateRendement(const2Converted);
			var const1Increment = await const1IncrementTask;

			var adjustmentLevel = (await (request.Tasks["AdjustmentLevel"] as Task<AdjustmentLevelResponse>)).AdjustmentLevel;

			var const1AdjustmentTask = CalculateWithFrameAsync<decimal, decimal>(const1Increment, (i, input, output) => output[i] = input[i] * adjustmentLevel);
			var const2AdjustmentTask = CalculateWithFrameAsync<decimal, decimal>(const2Increment, (i, input, output) => output[i] = input[i] * adjustmentLevel);

			var const1Adjustment = await const1AdjustmentTask;
			var const1BenchTask = GetBench(const1Adjustment);

			var const2Adjustment = await const2AdjustmentTask;
			var const2BenchTask = GetBench(const2Adjustment);

			return new BenchmarkLevelResponse
			{
				Constituent1 = await const1BenchTask,
				Constituent2 = await const2BenchTask
			};
		}
	}
}