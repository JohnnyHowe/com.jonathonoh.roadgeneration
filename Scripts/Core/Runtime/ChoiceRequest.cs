namespace JonathonOH.RoadGeneration
{
	public readonly struct ChoiceRequest
	{
		public readonly IChainGenerator ChainGenerator { get; init; }
		public readonly int MaxCheckDepth { get; init; }
	}
}
