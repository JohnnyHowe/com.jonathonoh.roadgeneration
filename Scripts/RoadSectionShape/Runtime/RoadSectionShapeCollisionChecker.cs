using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Core;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	/// <summary>
	/// TODO
	/// Error check when sections lists are too small! 0, 1, 2(?) items
	/// </summary>
	public class RoadSectionShapeCollisionChecker
	{
		public bool DebugDrawEnabled = true;
		private readonly Color collisionColor = Color.red;
		private readonly Color subjectColor = Color.yellow;
		private readonly Color checkedColor = Color.white;
		private readonly Color uncheckedColor = Color.grey;

		private ShapeCache shapeCache;

		public RoadSectionShapeCollisionChecker(ShapeCache shapeCache)
		{
			this.shapeCache = shapeCache;
		}

		public IRoadSection GetSectionLastCollidesWith(IReadOnlyList<IRoadSection> sections)
		{
			IRoadSection last = sections.Last();
			IReadOnlyList<IRoadSection> sectionsMinusLast = sections.Take(sections.Count - 1).ToList();
			return GetCollidingSectionOrNull(sectionsMinusLast, last);
		}

		public IRoadSection GetCollidingSectionOrNull(IReadOnlyList<IRoadSection> sectionsToCheck, IRoadSection subject)
		{
			RoadSectionShape subjectShape = shapeCache.GetShape(subject);
			List<RoadSectionShape> shapesToCheck = shapeCache.GetShapes(sectionsToCheck).ToList();

			TransformData start = ShapeAligner.DefaultStart;
			if (DebugDrawEnabled && shapesToCheck.Count > 0)
			{
				start = shapesToCheck[0].Start;
			}

			int indexOfSectionToCheckWithCollision = GetIndexOfShapeWithCollision(shapesToCheck, subjectShape, start);

			if (indexOfSectionToCheckWithCollision == -1)
			{
				return null;
			}
			else
			{
				return sectionsToCheck[indexOfSectionToCheckWithCollision];
			}
		}

		private int GetIndexOfShapeWithCollision(IEnumerable<RoadSectionShape> shapesToCheck, RoadSectionShape subject, TransformData startAlignment)
		{
			IReadOnlyList<RoadSectionShape> shapesAligned = ShapeAligner.GetAligned(startAlignment, shapesToCheck).ToList();
			RoadSectionShape subjectShapeAligned = ShapeAligner.GetAligned(shapesAligned.Last().End, subject);
			return GetIndexOfAlignedShapeWithCollision(shapesAligned, subjectShapeAligned);
		}

		private int GetIndexOfAlignedShapeWithCollision(IReadOnlyList<RoadSectionShape> shapesAligned, RoadSectionShape subjectShapeAligned)
		{
			List<int> collisionCheckOrder = CollisionCheckOrderer.GetCollisionCheckOrder(shapesAligned.Count).ToList();
			int indexOfShapeWithCollision = GetIndexOfAlignedShapeWithCollision(shapesAligned, subjectShapeAligned, collisionCheckOrder);

			if (DebugDrawEnabled)
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
