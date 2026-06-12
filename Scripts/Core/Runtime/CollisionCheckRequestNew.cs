using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.Core
{
	public struct CollisionCheckRequest
	{
		public readonly IRoadSection Subject { get; init; }
		public readonly IReadOnlyList<IRoadSection> AlreadyPlaced { get; init; }
		public readonly IReadOnlyList<IRoadSection> AllowedSections { get; init; }
		public readonly int MaxCheckDepth { get; init; }

		public IEnumerable<IRoadSection> GetFullChain()
		{
			foreach (IRoadSection section in AlreadyPlaced) yield return section;
			yield return Subject;
		}
	}
}

