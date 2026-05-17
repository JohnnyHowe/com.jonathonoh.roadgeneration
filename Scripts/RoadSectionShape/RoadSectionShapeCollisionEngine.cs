using JonathonOH.RoadGeneration.Collision;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class RoadSectionShapeCollisionEngine : ICollisionEngine
	{
		private const bool debugDraw = true;
		private readonly Color collisionColor = Color.red;
		private readonly Color subjectColor = Color.yellow;
		private readonly Color defaultColor = Color.white;

		private ShapeCache shapeCache;

		public CollisionCheckResult? GetResult()
		{
			throw new System.NotImplementedException();
		}

		public void Reset(CollisionCheckRequestNew request)
		{
			throw new System.NotImplementedException();
		}

		public void Step()
		{
			throw new System.NotImplementedException();
		}
	}
}
