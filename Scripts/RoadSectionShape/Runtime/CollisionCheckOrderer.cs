using System;
using System.Collections.Generic;
using System.Linq;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	/// <summary>
	/// Provides an ordering for checking road section collisions.
	/// </summary>
	public static class CollisionCheckOrderer
	{
		/// <summary>
		/// Iterates over range [0, sectionsToCheck.Count).
		/// Gets all values in that range, but not in order.
		/// </summary>
		public static IEnumerable<int> GetCollisionCheckOrder(int sectionsToCheck)
		{
			return GetCollisionCheckOrderZeroStart(sectionsToCheck).Reverse();
		}

		private static IEnumerable<int> GetCollisionCheckOrderZeroStart(int sectionsToCheck)
		{
			for (int i = 0; i < sectionsToCheck; i += 2)
			{
				yield return i;
			}
			for (int i = 1; i < sectionsToCheck; i += 2)
			{
				yield return i;
			}
		}
	}
}
