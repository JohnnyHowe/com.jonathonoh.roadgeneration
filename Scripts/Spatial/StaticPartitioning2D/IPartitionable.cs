using UnityEngine;

namespace JonathonOH.Spatial.StaticPartitioning2D
{
	public interface IPartitionable
	{
		public Rect GetBoundary();

		public bool Overlaps(IPartitionable other)
		{
			return GetBoundary().Overlaps(other.GetBoundary());
		}
	}
}
