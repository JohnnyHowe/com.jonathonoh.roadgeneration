
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.Collision
{
	public class RoadSectionShapeCollisionChecker : ICollisionChecker
	{
		public CollisionCheckResult CheckOneAgainstMany(RoadSection subject, IEnumerable<RoadSection> alreadyAligned, IEnumerable<RoadSection> toAlign)
		{
			throw new System.NotImplementedException();
		}
	}
}
