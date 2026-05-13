using System.Collections.Generic;
using System.Linq;

namespace JonathonOH.RoadGeneration.Collision
{
	public class RoadSectionShapeCollisionChecker : ICollisionChecker
	{
		public CollisionCheckResult CheckOneAgainstMany(RoadSection subject, IEnumerable<RoadSection> alreadyPlaced, IEnumerable<RoadSection> candidates)
		{
			List<RoadSection> alreadyPlacedList = alreadyPlaced.ToList();
			List<RoadSection> candidatesList = candidates.ToList();

			List<RoadSectionShape> shapesToCheckAgainstAligned = GetShapesAligned(alreadyPlacedList, candidatesList).ToList();
			RoadSectionShape subjectShapeAligned = GetShapeAligned(shapesToCheckAgainstAligned.Last(), subject);

			int shapeCausingCollisionIndex = GetIndexOfShapeWithOverlap(subjectShapeAligned, shapesToCheckAgainstAligned);

			// No collision
			if (shapeCausingCollisionIndex == -1)
			{
				return CollisionCheckResult.CreateWithoutCollision(subject);
			}

			// Collides with already placed section
			if (shapeCausingCollisionIndex < alreadyPlacedList.Count)
			{
				return CollisionCheckResult.CreateWithCollision(subject, alreadyPlacedList[shapeCausingCollisionIndex]);
			}

			// Collides with candidate section
			int shapeCausingCollisionIndexInCandidates = shapeCausingCollisionIndex - alreadyPlacedList.Count;
			return CollisionCheckResult.CreateWithCollision(subject, candidatesList[shapeCausingCollisionIndexInCandidates]);
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
			return shape1.DoesOverlapWith(shape2);
		}
	}
}
