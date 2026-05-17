using JonathonOH.Spatial;
using UnityEngine;

namespace JonathonOH.RoadGeneration.Core
{
	public interface IRoadSection
	{
		public TransformData Entry { get; }
		public TransformData Exit { get; }
		public Mesh GetBoundaryInEntrySpace();
		public string GetShapeId();
	}
}
