using System.Collections.Generic;

namespace JonathonOH.RoadGeneration
{
	public interface ICollisionChecker
	{
		public CollisionCheckResult CheckOneAgainstMany(CollisionCheckRequest request);
	}
}
