using UnityEngine;

namespace JonathonOH.RoadGeneration.Core
{
	public interface IRoadSection
	{
		public Pose Entry { get; }
		public Pose Exit { get; }
		public bool IsBoundaryInfiniteHeight { get; }
		public Mesh GetBoundaryInEntrySpace();
		public string GetShapeId();
	}
}
