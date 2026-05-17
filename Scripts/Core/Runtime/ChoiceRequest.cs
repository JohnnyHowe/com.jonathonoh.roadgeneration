
using System.Collections.Generic;
using JonathonOH.RoadGeneration.Core;

namespace JonathonOH.RoadGeneration.ChoiceEngine
{
	public readonly struct ChoiceRequest
	{
		public readonly IReadOnlyList<IRoadSection> CurrentSectionsInWorld { get; init; }
		public readonly IReadOnlyList<IRoadSection> SectionsInPreferenceOrder { get; init; }
		public readonly int MaxCheckDepth { get; init; }
	}
}
