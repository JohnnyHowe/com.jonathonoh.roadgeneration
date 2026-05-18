using UnityEngine;

namespace JonathonOH.Spatial.StaticPartitioning2D
{
	public interface IPartitionable
	{
		public Rect GetBoundary();
	}
}
