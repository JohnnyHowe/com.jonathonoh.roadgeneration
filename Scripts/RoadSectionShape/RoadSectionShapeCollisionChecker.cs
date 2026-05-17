using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	/// <summary>
	/// TODO
	/// Error check when sections lists are too small! 0, 1, 2(?) items
	/// </summary>
	public class RoadSectionShapeCollisionChecker
	{
		private const bool debugDraw = true;
		private readonly Color collisionColor = Color.red;
		private readonly Color subjectColor = Color.yellow;
		private readonly Color defaultColor = Color.white;

		private ShapeCache shapeCache;

		public RoadSectionShapeCollisionChecker(ShapeCache shapeCache)
		{
			this.shapeCache = shapeCache;
		}

		public IRoadSectionShapeCollisionCheckable GetSectionLastCollidesWith(IReadOnlyList<IRoadSectionShapeCollisionCheckable> sections)
		{
			IRoadSectionShapeCollisionCheckable last = sections.Last();
			IReadOnlyList<IRoadSectionShapeCollisionCheckable> sectionsMinusLast = sections.Take(sections.Count - 1).ToList();
			return GetCollidingSectionOrNull(sectionsMinusLast, last);
		}

		public IRoadSectionShapeCollisionCheckable GetCollidingSectionOrNull(IReadOnlyList<IRoadSectionShapeCollisionCheckable> sectionsToCheck, IRoadSectionShapeCollisionCheckable subject)
		{
			RoadSectionShape subjectShape = shapeCache.GetShape(subject);
			IEnumerable<RoadSectionShape> shapesToCheck = shapeCache.GetShapes(sectionsToCheck);

			int indexOfSectionToCheckWithCollision = GetIndexOfShapeWithCollision(shapesToCheck, subjectShape);

			if (indexOfSectionToCheckWithCollision == -1)
			{
				return null;
			}
			else
			{
				return sectionsToCheck[indexOfSectionToCheckWithCollision];
			}
		}

		/// <summary>
		/// Returns -1 if subject does not collide with any.
		/// </summary>
		private int GetIndexOfShapeWithCollision(IEnumerable<RoadSectionShape> shapesToCheck, RoadSectionShape subject)
		{
			IReadOnlyList<RoadSectionShape> shapesAligned = ShapeAligner.GetAligned(shapesToCheck).ToList();
			RoadSectionShape subjectShapeAligned = ShapeAligner.GetAligned(shapesAligned.Last().End, subject);
			return GetIndexOfAlignedShapeWithCollision(shapesAligned, subjectShapeAligned);
		}

		/// <summary>
		/// Returns -1 if no collision.
		/// </summary>
		private int GetIndexOfAlignedShapeWithCollision(IReadOnlyList<RoadSectionShape> shapesAligned, RoadSectionShape subjectShapeAligned)
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
