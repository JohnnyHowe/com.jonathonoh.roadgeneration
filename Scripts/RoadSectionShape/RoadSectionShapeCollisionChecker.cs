using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Collision;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class RoadSectionShapeCollisionChecker : ICollisionChecker
	{
		private const bool debugDraw = true;
		private ShapeCache shapeCache;

		public RoadSectionShapeCollisionChecker()
		{
			shapeCache = new ShapeCache();
		}

		public CollisionCheckResult CheckOneAgainstMany(CollisionCheckRequest request)
		{
			List<RoadSectionShape> shapesToCheckAgainstAligned = GetShapesAligned(request.AlreadyPlaced, request.Candidates).ToList();
			RoadSectionShape subjectShapeAligned = ShapeAligner.GetAligned(shapesToCheckAgainstAligned.Last().End, request.Subject.GetShape());

			int shapeCausingCollisionIndex = GetIndexOfShapeWithOverlap(subjectShapeAligned, shapesToCheckAgainstAligned);

			// No collision
			if (shapeCausingCollisionIndex == -1)
			{
				return new CollisionCheckResult()
				{
					Request = request,
					HasCollision = false,
					CollidesWith = null
				};
			}

			RoadSection collidingSection;

			// Collides with already placed section
			if (shapeCausingCollisionIndex < request.AlreadyPlaced.Count)
			{
				collidingSection = request.AlreadyPlaced[shapeCausingCollisionIndex];
			}
			else
			{
				collidingSection = request.Candidates[shapeCausingCollisionIndex - request.AlreadyPlaced.Count];
			}

			return new CollisionCheckResult()
			{
				Request = request,
				HasCollision = true,
				CollidesWith = collidingSection
			};
		}

		/// <summary>
		/// -1 if no overlap
		/// </summary>
		private int GetIndexOfShapeWithOverlap(RoadSectionShape subject, List<RoadSectionShape> toCheckAgainst)
		{
			if (debugDraw) subject.DebugDraw(Color.red);
			for (int i = 0; i < toCheckAgainst.Count(); i++)
			{
				if (AreColliding(subject, toCheckAgainst[i]))
				{
					if (debugDraw) toCheckAgainst[i].DebugDraw(Color.red);
					return i;
				}
				else if (debugDraw)
				{
					toCheckAgainst[i].DebugDraw(Color.white);
				}
			}
			return -1;
		}

		private IEnumerable<RoadSectionShape> GetShapesAligned(IEnumerable<RoadSection> alreadyPlaced, IEnumerable<RoadSection> candidates)
		{
			return ShapeAligner.GetAllAligned(
				alreadyPlaced.Select(section => section.GetShape()).ToList(),
				candidates.Select(section => section.GetShape())
			);
		}

		private bool AreColliding(RoadSectionShape shape1, RoadSectionShape shape2)
		{
			return shape1.DoesOverlapWith(shape2);
		}
	}
}
