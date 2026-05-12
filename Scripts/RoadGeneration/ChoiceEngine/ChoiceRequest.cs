
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.ChoiceEngine
{
	internal readonly struct ChoiceRequest
	{
		public readonly IReadOnlyList<RoadSection> CurrentSectionsInWorld { get; init; }
		public readonly IReadOnlyList<RoadSection> SectionsInPreferenceOrder { get; init; }
		public readonly int MaxCheckDepth { get; init; }
	}
}
