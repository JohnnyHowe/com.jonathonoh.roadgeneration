namespace JonathonOH.RoadGeneration
{
	/// <summary>
	/// Get all the valid permutations of IRoadSections.
	/// What is valid and the ordering is up to the implementation.
	/// </summary>
	public interface IChainGenerator
	{
		public Chain Current { get; }
		public void Extend();
		public bool Next();
	}
}
