namespace JonathonOH.RoadGeneration.ChoiceEngine
{
	public readonly struct ChoiceResult
	{
		public readonly bool ChoiceFound { get; init; }
		public readonly RoadSection ChosenSection { get; init; }
		public readonly string FailureReason { get; init; }
	}
}
