using UnityEngine;

namespace JonathonOH.Spatial.Partitioning2D
{
	public interface IPartitionable
	{
		public Rect GetBoundary();
	}
}
