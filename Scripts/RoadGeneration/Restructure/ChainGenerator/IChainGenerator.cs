namespace JonathonOH.RoadGeneration.ChainGenerator
{
	public interface IChainGenerator
	{
		public RoadSection CurrentCandidate { get; }
		public void StepValid();
		public void StepInvalid();
		public bool HasFoundSolution();
		public bool AreOptionsExhausted();
	}
}
