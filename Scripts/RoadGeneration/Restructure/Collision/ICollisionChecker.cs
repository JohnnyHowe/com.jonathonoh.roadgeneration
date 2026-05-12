using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.Collision
{
	public interface ICollisionChecker
	{
		public CollisionCheckResult CheckOneAgainstMany(RoadSection subject, IEnumerable<RoadSection> alreadyAligned, IEnumerable<RoadSection> candidates);
	}
}
