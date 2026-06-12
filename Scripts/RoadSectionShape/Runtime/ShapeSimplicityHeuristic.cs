using System;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class ShapeSimplicityHeuristic
	{

		/// <summary>
		/// Higher value = likely more effective to check early.
		/// 
		/// Things taken into account:
		/// - angle difference between start and end (less is better)
		/// </summary>
		public float GetHeuristic(RoadSectionShape shape)
		{
			throw new NotImplementedException();
		}
	}
}
