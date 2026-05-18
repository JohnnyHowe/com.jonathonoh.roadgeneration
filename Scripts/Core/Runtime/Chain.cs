using System.Collections.Generic;

namespace JonathonOH.RoadGeneration
{
	public struct Chain
	{
		public readonly bool IsCandidatesAtTargetLength { get; init; }
		public readonly IReadOnlyList<IRoadSection> AlreadyPlaced { get; init; }
		public readonly IReadOnlyList<IRoadSection> Candidates { get; init; }

		public override string ToString()
		{
			return $"Chain< AlreadyPlaced={string.Join(", ", AlreadyPlaced)}, Candidates={string.Join(", ", Candidates)} >";
		}
	}
}
