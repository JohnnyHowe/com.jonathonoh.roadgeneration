using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
	public interface IRoadSection
	{
		public Pose Entry { get; }
		public Pose Exit { get; }
		public bool IsBoundaryInfiniteHeight { get; }
		public IEnumerable<Vector3> GetBoundaryPoints();
		public string GetShapeId();
	}
}
