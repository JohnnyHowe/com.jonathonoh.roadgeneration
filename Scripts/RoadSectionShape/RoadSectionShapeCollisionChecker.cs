using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.Collision
{
	public class RoadSectionShapeCollisionChecker : ICollisionChecker
	{
		public CollisionCheckResult CheckOneAgainstMany(CollisionCheckRequest request)
		{
			List<RoadSectionShape> shapesToCheckAgainstAligned = GetShapesAligned(request.AlreadyPlaced, request.Candidates).ToList();
			RoadSectionShape subjectShapeAligned = GetShapeAligned(shapesToCheckAgainstAligned.Last(), request.Subject);

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
			for (int i = 0; i < toCheckAgainst.Count(); i++)
			{
				if (AreColliding(subject, toCheckAgainst[i]))
				{
					return i;
				}
			}
			return -1;
		}

		private IEnumerable<RoadSectionShape> GetShapesAligned(IEnumerable<RoadSection> alreadyPlaced, IEnumerable<RoadSection> candidates)
		{
			TransformData previousSectionEnd = TransformData.Default();

			foreach (RoadSection section in alreadyPlaced)
			{
				RoadSectionShape sectionShape = section.GetShape();
				previousSectionEnd = sectionShape.End;
				yield return sectionShape;
			}

			foreach (RoadSection section in candidates)
			{
				RoadSectionShape sectionShape = GetShapeAligned(previousSectionEnd, section);
				previousSectionEnd = sectionShape.End;
				yield return sectionShape;
			}
		}

		private RoadSectionShape GetShapeAligned(RoadSectionShape previous, RoadSection toAlign)
		{
			return GetShapeAligned(previous.End, toAlign);
		}

		private RoadSectionShape GetShapeAligned(TransformData start, RoadSection toAlign)
		{
			return toAlign.GetShape().GetTranslatedCopy(start);
		}

		private bool AreColliding(RoadSectionShape shape1, RoadSectionShape shape2)
		{
			shape1.DebugDraw(Color.white);
			shape2.DebugDraw(Color.yellow);
			return shape1.DoesOverlapWith(shape2);
		}
	}
}
