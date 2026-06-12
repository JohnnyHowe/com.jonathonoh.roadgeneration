using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.Core
{
	public readonly struct ChoiceRequest
	{
		public readonly IReadOnlyList<IRoadSection> CurrentSectionsInWorld { get; init; }
		public readonly IReadOnlyList<IRoadSection> SectionsInPreferenceOrder { get; init; }
		public readonly int MaxCheckDepth { get; init; }
	}
}
