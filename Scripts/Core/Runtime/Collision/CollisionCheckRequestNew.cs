using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Core;

namespace JonathonOH.RoadGeneration.Collision
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

