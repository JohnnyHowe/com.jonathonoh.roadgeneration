namespace JonathonOH.RoadGeneration.ChoiceEngine
{
	public readonly struct ChoiceResult
	{
		public enum ChoiceFailureReason
		{
			NoChoiceFound,
			NoFailure
		}

		public readonly bool IsChoiceFound { get; init; }
		public readonly RoadSection ChosenSection { get; init; }
		public readonly ChoiceFailureReason FailureReason { get; init; }
	}
}
