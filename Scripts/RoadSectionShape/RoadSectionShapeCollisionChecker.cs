using System;
using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Collision;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class RoadSectionShapeCollisionChecker : ICollisionChecker
	{
		private const bool debugDraw = true;
		private readonly Color collisionColor = Color.red;
		private readonly Color subjectColor = Color.yellow;
		private readonly Color defaultColor = Color.white;

		private ShapeCache shapeCache;

		public RoadSectionShapeCollisionChecker()
		{
			shapeCache = new ShapeCache();
		}

		public CollisionCheckResult CheckOneAgainstMany(CollisionCheckRequest request)
		{
			throw new NotImplementedException();
			// RoadSection collidingSection = GetCollidingSection(request);

			// return new CollisionCheckResult()
			// {
			// 	Request = request,
			// 	HasCollision = collidingSection != null,
			// 	CollidesWith = collidingSection
			// };
		}

		/// <summary>
		/// Returns null if no colliding section
		/// </summary>
		private RoadSection GetCollidingSection(CollisionCheckRequest request)
		{
			IReadOnlyList<RoadSection> sections = request.GetFullChain().ToList();

			if (sections.Count == 0)
			{
				return null;
			}

			IReadOnlyList<RoadSectionShape> shapes = shapeCache.GetShapes(sections).ToList();

			// TODO use the commented out version. Other one is temp for debug
			// IReadOnlyList<RoadSectionShape> shapesAligned = ShapeAligner.GetAligned(shapes).ToList();
			IReadOnlyList<RoadSectionShape> shapesAligned = ShapeAligner.GetAligned(shapes[0].Start, shapes).ToList();

			RoadSectionShape subjectShape = shapeCache.GetShape(request.Subject);
			RoadSectionShape subjectShapeAligned = ShapeAligner.GetAligned(shapesAligned.Last().End, subjectShape);

			int overlappingShapeIndex = GetIndexOfSectionWithCollision(shapesAligned, subjectShapeAligned);

			if (overlappingShapeIndex == -1)
			{
				return null;
			}
			else
			{
				return sections[overlappingShapeIndex];
			}
		}

		/// <summary>
		/// Returns -1 if no collision.
		/// </summary>
		private int GetIndexOfSectionWithCollision(IReadOnlyList<RoadSectionShape> shapesAligned, RoadSectionShape subjectShapeAligned)
		{
			if (debugDraw) subjectShapeAligned.DebugDraw(subjectColor);

			// Reverse search beacuse we're more likely to overlap with something recent.
			for (int i = shapesAligned.Count - 1; i >= 0; i--)
			{
				RoadSectionShape shapeToCheckAgainst = shapesAligned[i];
				if (AreColliding(shapeToCheckAgainst, subjectShapeAligned))
				{
					if (debugDraw) shapesAligned[i].DebugDraw(collisionColor);
					return i;
				}
				else
				{
					if (debugDraw) shapesAligned[i].DebugDraw(defaultColor);
				}
			}
			return -1;
		}

		private bool AreColliding(RoadSectionShape shape1, RoadSectionShape shape2)
		{
			return shape1.DoesOverlapWith(shape2);
		}
	}
}
