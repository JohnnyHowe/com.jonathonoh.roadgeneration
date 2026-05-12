using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Other;
using JonathonOH.RoadGeneration.ChoiceEngine;

namespace JonathonOH.RoadGeneration
{
	/// <summary>
	/// Must call Reset() before anything else
	/// </summary>
	public class RoadGeneratorChoiceEngine
	{
		public DFSCombinationGenerator _combinationGenerator;
		public ChoiceRequest CurrentChoiceRequest { get; private set; }
		public ChoiceResult CurrentChoiceResult { get; private set; }
		public bool IsSearching { get; private set; } = false;

		private const int MAX_ITERATIONS = 10000000;

		public void Reset(ChoiceRequest choiceRequest)
		{
			CurrentChoiceRequest = choiceRequest;
			_combinationGenerator = new DFSCombinationGenerator(choiceRequest.SectionsInPreferenceOrder.Count, choiceRequest.MaxCheckDepth);
			IsSearching = true;
		}

		public void StepUntilChoiceIsFound()
		{
			for (int i = 0; i < MAX_ITERATIONS; i++)
			{
				if (_combinationGenerator.IsImpossible()) break;
				if (HasFoundChoice()) break;
				Step();
			}
		}

		public void Step()
		{
			if (!IsSearching)
			{
				return;
			}

			RunCollisionCheckAndStepCombinationGenerator();
			CheckCombinationTermination();
		}

		private void CheckCombinationTermination()
		{
			if (_combinationGenerator.HasFoundSolution())
			{
				IsSearching = false;
				Debug.Log($"Solution found! {CurrentChoiceResult}");
			}
			else if (_combinationGenerator.IsImpossible())
			{
				IsSearching = false;
			}
		}

		private void RunCollisionCheckAndStepCombinationGenerator()
		{
			RoadSection sectionCausingCollision = GetTheSectionCurrentCandidateCollidesWithWhenAligned();
			if (sectionCausingCollision != null)
			{
				Debug.Log($"Current candidate {GetCandidateRoadSections().Last()} overlaps with {sectionCausingCollision}");
				_combinationGenerator.StepInvalid();
			}
			else
			{
				_combinationGenerator.StepValid();
			}
		}

		/// <summary>
		/// Null if no collision.
		/// </summary>
		private RoadSection GetTheSectionCurrentCandidateCollidesWithWhenAligned()
		{
			var allCandidates = GetCandidateRoadSections();
			var allCandidateShapes = GetCandidatesAligned(allCandidates);

			RoadSectionShape currentCandidateShape = allCandidateShapes.Last();

			// Check against sections in world 
			foreach (RoadSection roadSection in CurrentChoiceRequest.CurrentSectionsInWorld.Reverse())
			{
				RoadSectionShape toCheck = roadSection.GetShape();
				if (currentCandidateShape.DoesOverlapWith(toCheck))
				{
					return roadSection;
				}
			}

			// Check against other candidates
			for (int i = 0; i < allCandidateShapes.Count - 1; i++) // -1 because the final IS the subject
			{
				RoadSectionShape toCheck = allCandidateShapes[i];
				if (currentCandidateShape.DoesOverlapWith(toCheck))
				{
					return allCandidates[i];
				}
			}

			return null;
		}

		private List<RoadSectionShape> GetCandidatesAligned(IEnumerable<RoadSection> candidateRoadSections)
		{
			// Figuring out the architecture so this method could exist was a nightmare.
			// Both big redesigns were a result of this.
			// I hope it looks obvious and easy to make yourself - that means I've done it right
			List<RoadSectionShape> alignedCandidates = new List<RoadSectionShape>();
			TransformData nextStartPoint = GetFirstCandidateStartPoint();
			foreach (RoadSection candidateSection in candidateRoadSections)
			{
				RoadSectionShape alignedCandidateShape = candidateSection.GetShape().GetTranslatedCopy(nextStartPoint);
				alignedCandidates.Add(alignedCandidateShape);
				nextStartPoint = alignedCandidateShape.End;
			}
			return alignedCandidates;
		}

		private List<RoadSection> GetCandidateRoadSections()
		{
			List<RoadSection> candidates = new List<RoadSection>();
			foreach (int candidateChoiceIndex in _combinationGenerator.GetState())
			{
				if (candidateChoiceIndex == -1) break;
				candidates.Add(CurrentChoiceRequest.SectionsInPreferenceOrder[candidateChoiceIndex]);
			}
			return candidates;
		}

		private TransformData GetFirstCandidateStartPoint()
		{
			// TODO this is copied in RoadGenerator - they both define start points - should not be separate
			var sectionsInWorld = CurrentChoiceRequest.CurrentSectionsInWorld;
			if (sectionsInWorld.Count == 0)
			{
				return new TransformData(Vector3.zero, Quaternion.Euler(0, 0, 1), Vector3.one);
			}
			return sectionsInWorld[sectionsInWorld.Count - 1].GetShape().End;

		}

		public bool HasFoundChoice()
		{
			return _combinationGenerator.HasFoundSolution();
		}

		internal class NoChoiceFoundException : Exception { }

		public RoadSection GetChoicePrototype()
		{
			if (!HasFoundChoice())
			{
				throw new NoChoiceFoundException();
			}
			return CurrentChoiceRequest.SectionsInPreferenceOrder[_combinationGenerator.GetState()[0]];
		}
	}
}
