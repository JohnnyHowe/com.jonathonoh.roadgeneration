namespace JonathonOH.RoadGeneration.ChoiceEngine
{
	internal readonly struct ChoiceResult
	{
		public readonly bool ChoiceFound { get; init; }
		public readonly RoadSection ChosenSection { get; init; }
	}
}
