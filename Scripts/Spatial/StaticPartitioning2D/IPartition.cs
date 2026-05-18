using System.Collections.Generic;

namespace JonathonOH.Spatial.StaticPartitioning2D
{
	public interface IPartition
	{
		public void SetAllObjects(IEnumerable<IPartitionable> objects);
		public void Add(IPartitionable partitionable);
		public void Remove(IPartitionable partitionable);
		public IEnumerable<IPartitionable> GetObjectsOverlapping(IPartitionable other);
	}
}
