using System;
using System.Collections.Generic;

namespace JonathonOH.Spatial.StaticPartitioning2D
{
	public class BSP: IPartition
	{
		public void SetAllObjects(IEnumerable<IPartitionable> objects)
		{
			throw new NotImplementedException();
		}

		public void Add(IPartitionable partitionable)
		{
			throw new NotImplementedException();
		}

		public void Remove(IPartitionable partitionable)
		{
			throw new NotImplementedException();
		}

		public IPartitionable GetFirstObjectWithOverlap(IPartitionable other)
		{
			throw new NotImplementedException();
		}
	}
}
