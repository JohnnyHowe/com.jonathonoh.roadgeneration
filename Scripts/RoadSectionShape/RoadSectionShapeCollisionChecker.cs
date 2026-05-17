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
		private readonly Color checkedColor = Color.white;
		private readonly Color uncheckedColor = Color.grey;

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

		private int GetIndexOfAlignedShapeWithCollision(IReadOnlyList<RoadSectionShape> shapesAligned, RoadSectionShape subjectShapeAligned)
		{
			List<int> collisionCheckOrder = CollisionCheckOrderer.GetCollisionCheckOrder(shapesAligned.Count).ToList();
			int indexOfShapeWithCollision = GetIndexOfAlignedShapeWithCollision(shapesAligned, subjectShapeAligned, collisionCheckOrder);

			if (debugDraw)
			{
				subjectShapeAligned.DebugDraw(subjectColor);

				bool hasPassedShapeWithCollision = false;
				foreach (int checkIndex in collisionCheckOrder)
				{
					if (indexOfShapeWithCollision == checkIndex)
					{
						hasPassedShapeWithCollision = true;
					}
					Color color = indexOfShapeWithCollision == checkIndex ? collisionColor : hasPassedShapeWithCollision ? uncheckedColor : checkedColor;
					shapesAligned[checkIndex].DebugDraw(color);
				}
			}

			return indexOfShapeWithCollision;
		}

		/// <summary>
		/// Returns -1 if no collision.
		/// </summary>
		private int GetIndexOfAlignedShapeWithCollision(IReadOnlyList<RoadSectionShape> shapesAligned, RoadSectionShape subjectShapeAligned, IEnumerable<int> checkOrder)
		{
			foreach (int shapeIndexToCheck in checkOrder)
			{
				RoadSectionShape shapeToCheckAgainst = shapesAligned[shapeIndexToCheck];
				if (AreColliding(shapeToCheckAgainst, subjectShapeAligned))
				{
					return shapeIndexToCheck;
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
