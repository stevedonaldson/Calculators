using System.Collections.Generic;

namespace Calculator
{
	public class BenchmarkRequest
	{
		public decimal[] Constituent1 { get; set; }
		public decimal[] Constituent2 { get; set; }
		public decimal[] IndexProxy { get; set; }
		public decimal[] FXRate { get; set; }
		public decimal Const1Weight { get; set; }
		public decimal Const2Weight { get; set; }
		public decimal AssetClassLeverageLongTermTargetLTV { get; set; }
		public decimal AssetClassLeverageLTVImpliedInProxies { get; set; }
	}

	public class BenchmarkResponse
	{
		public decimal[] Bench1 { get; set; }
		public decimal[] Bench2 { get; set; }
	}

	public class BenchmarkExtrapolateHistoryRequest
	{
		public Dictionary<string, object> Tasks { get; set; }
		public decimal[] Constituent { get; set; }
		public decimal[] Index { get; set; }
	}

	public class BenchmarkExtrapolateHistoryResponse
	{
		public decimal[] Constituent { get; set; }
	}

	public class IndexIncrementsRequest
	{
		public decimal[] Data { get; set; }
	}

	public class IndexIncrementsResponse
	{
		public decimal[] PercentIncrements { get; set; }
	}

	public class LocalBenchmarkRequest
	{
		public Dictionary<string, object> Tasks { get; set; }
		public decimal[] Constituent1 { get; set; }
		public decimal Const1Weight { get; set; }
		public decimal Const2Weight { get; set; }
	}

	public class LocalBenchmarkResponse
	{
		public decimal[] Data { get; set; }
	}

	public class AdjustmentLevelRequest
	{
		public decimal AssetClassLeverageLongTermTargetLTV { get; set; }
		public decimal AssetClassLeverageLTVImpliedInProxies { get; set; }
	}

	public class AdjustmentLevelResponse
	{
		public decimal AdjustmentLevel { get; set; }
	}

	public class BenchmarkLevelRequest
	{
		public Dictionary<string, object> Tasks { get; set; }
		public decimal[] Constituent1 { get; set; }
		public decimal[] FxRate { get; set; }
	}

	public class BenchmarkLevelResponse
	{
		public decimal[] Constituent1 { get; set; }
		public decimal[] Constituent2 { get; set; }
	}
}