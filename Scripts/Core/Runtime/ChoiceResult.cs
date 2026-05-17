using JonathonOH.RoadGeneration.Core;

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
		public readonly IRoadSection ChosenSection { get; init; }
		public readonly ChoiceFailureReason FailureReason { get; init; }
	}
}
