using System.Collections.Generic;

namespace JonathonOH.RoadGeneration
{
	/// <summary>
	/// Get all the valid permutations of IRoadSections.
	/// What is valid and the ordering is up to the implementation.
	/// </summary>
	public interface IChainGenerator: IEnumerator<Chain>
	{
		public bool MoveNextInvalid();
	}
}
