
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration
{
	public struct CollisionCheckRequest
	{
		public readonly RoadSection Subject { get; init; }
		public readonly IReadOnlyList<RoadSection> AlreadyPlaced { get; init; }
		public readonly IReadOnlyList<RoadSection> Candidates { get; init; }

		public IEnumerable<RoadSection> GetFullChain()
		{
			foreach (RoadSection section in AlreadyPlaced) yield return section;
			foreach (RoadSection section in Candidates) yield return section;
			yield return Subject;
		}
	}
}
