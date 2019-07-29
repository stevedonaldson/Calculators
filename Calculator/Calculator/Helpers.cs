using System;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator
{
	public static class Helpers
	{
		public static long CalculateSum(int[] nums)
		{
			long total = 0;

			Parallel.For<long>(
				0,
				nums.Length,
				() => 0,
				(index, state, subTotal) =>
				{
					subTotal += nums[index];
					return subTotal;
				},
				(subTotal) => Interlocked.Add(ref total, subTotal));

			return total;
		}

		public static Task<long> CalculateSumAsync(int[] nums)
		{
			return Task.Run(() => CalculateSum(nums));
		}

		public static decimal CalculateSum(decimal[] nums, int decimalPlaces)
		{
			long total = 0;
			decimal multiplier = 1M;

			for (int i = 0; i < decimalPlaces; i++)
			{
				multiplier *= 10;
			}

			Parallel.For<long>(
				0, 
				nums.Length, 
				() => 0, 
				(index, state, subTotal) =>
				{
					subTotal += decimal.ToInt64(nums[index] * multiplier);
					return subTotal;
				},
				(subTotal) => Interlocked.Add(ref total, subTotal));

			return total / multiplier;
		}

		public static decimal CalculateSum(decimal[] nums)
		{
			decimal total = 0;

			for (int i = 0; i < nums.Length; i++)
			{
				total += nums[i];
			}

			return total;
		}

		public static Task<decimal> CalculateSumAsync(decimal[] nums, int decimalPlaces)
		{
			return Task.Run(() => CalculateSum(nums, decimalPlaces));
		}

		public static Task<decimal> CalculateSumAsync(decimal[] nums)
		{
			return Task.Run(() => CalculateSum(nums));
		}

		public static decimal CalculateAverge(int[] nums)
		{
			var sum = CalculateSum(nums);
			return sum / nums.Length;
		}

		public static async Task<decimal> CalculateAvergeAsync(int[] nums)
		{
			var sum = await CalculateSumAsync(nums);
			return sum / nums.Length;
		}

		public static decimal CalculateAverge(decimal[] nums, int decimalPlaces)
		{
			var sum = CalculateSum(nums, decimalPlaces);
			return sum / nums.Length;
		}

		public static decimal CalculateAverge(decimal[] nums)
		{
			var sum = CalculateSum(nums);
			return sum / nums.Length;
		}

		public static async Task<decimal> CalculateAvergeAsync(decimal[] nums, int decimalPlaces)
		{
			var sum = await CalculateSumAsync(nums, decimalPlaces);
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
	}
}