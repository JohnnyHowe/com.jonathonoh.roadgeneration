using System.Collections.Generic;

namespace JonathonOH.Spatial.StaticPartitioning2D
{
	public class NoPartition: IPartition
	{
		private HashSet<IPartitionable> objects = new HashSet<IPartitionable>();

		public void SetAllObjects(IEnumerable<IPartitionable> newObjects)
		{
			objects = new HashSet<IPartitionable>(newObjects);
		}

		public void Add(IPartitionable partitionable)
		{
			objects.Add(partitionable);
		}

		public void Remove(IPartitionable partitionable)
		{
			objects.Remove(partitionable);
		}

		public IEnumerable<IPartitionable> GetObjectsOverlapping(IPartitionable other)
		{
			foreach (IPartitionable obj in objects)
			{
				if (other.Overlaps(obj))
				{
					yield return obj;
				}
			}
		}
	}
}
