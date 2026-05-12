using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Other;

namespace JonathonOH.RoadGeneration
{
	/// <summary>
	/// Must call Reset() before anything else
	/// </summary>
	public class RoadGeneratorChoiceEngine
	{
		public class NoChoiceFoundException : Exception { }

		public DFSCombinationGenerator _combinationGenerator;
		private List<RoadSection> sectionsInWorld;
		private List<RoadSection> sectionPrototypes;

		private const int MAX_ITERATIONS = 10000000;
		private bool _impossible;

		public void Reset(List<RoadSection> currentPiecesInWorld, List<RoadSection> possibleChoicesInPreferenceOrder, int checkDepth)
		{
			Debug.Log("RoadGenerationChoiceEngine Reset");
			_combinationGenerator = new DFSCombinationGenerator(possibleChoicesInPreferenceOrder.Count, checkDepth);
			sectionsInWorld = currentPiecesInWorld;
			sectionPrototypes = possibleChoicesInPreferenceOrder;
			_impossible = false;
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
			if (_combinationGenerator.HasFoundSolution())
			{
				return;
			}
			if (_combinationGenerator.IsImpossible())
			{
				return;
			}

			Debug.Log("About to call _DoesLastCandidateSectionOverlapWithOthers()");
			if (_DoesLastCandidateSectionOverlapWithOthers())
			{
				Debug.Log("FUCK Invalid Path Hit\n" + string.Join(", ", GetFullChainToCheck().Select((section) => section.gameObject.name)));
				_combinationGenerator.StepInvalid();
			}
			else
			{
				_combinationGenerator.StepValid();
			}
			Debug.Log("Step finished");
		}

		private bool _DoesLastCandidateSectionOverlapWithOthers()
		{
			Debug.Log("Checking overlap");

			List<RoadSectionShape> allPiecesAligned = _GetCandidatesAndCurrentPiecesInWorldAligned();
			// TODO can allPiecesAligned.Count ever be zero? If so, prevent it from breaking
			RoadSectionShape currentCandidate = allPiecesAligned[allPiecesAligned.Count - 1];

			// foreach (RoadSectionShape worldRoadSectionShape in allPiecesAligned.Take(allPiecesAligned.Count - 1))
			for (int i = 0; i < allPiecesAligned.Count - 1; i++)
			{
				RoadSectionShape toCheck = allPiecesAligned[i];

				if (currentCandidate.DoesOverlapWith(toCheck))
				{
					Debug.Log($"Most recent section overlaps with current index {i}");
					return true;
				}
			}

			Debug.Log("Passed overlap check!");
			return false;
		}

		/// <summary>
		/// Current sections in world + sections we are going to check
		/// </summary>
		private IEnumerable<RoadSection> GetFullChainToCheck()
		{
			return sectionsInWorld.Concat(GetCandidatesNotAligned());
		}

		private List<RoadSectionShape> _GetCandidatesAndCurrentPiecesInWorldAligned()
		{
			return _GetCurrentPiecesInWorldShapes().Concat(GetCandidatesAligned()).ToList();
		}

		private List<RoadSectionShape> _GetCurrentPiecesInWorldShapes()
		{
			return sectionsInWorld.Select(section => section.GetShape()).ToList();
		}

		private List<RoadSectionShape> GetCandidatesAligned()
		{
			// Figuring out the architecture so this method could exist was a nightmare.
			// Both big redesigns were a result of this.
			// I hope it looks obvious and easy to make yourself - that means I've done it right
			List<RoadSectionShape> alignedCandidates = new List<RoadSectionShape>();
			TransformData nextStartPoint = _GetFirstCandidateStartPoint();
			foreach (RoadSection candidateSection in GetCandidatesNotAligned())
			{
				RoadSectionShape alignedCandidateShape = candidateSection.GetShape().GetTranslatedCopy(nextStartPoint);
				alignedCandidates.Add(alignedCandidateShape);
				nextStartPoint = alignedCandidateShape.End;
			}
			return alignedCandidates;
		}

		private List<RoadSection> GetCandidatesNotAligned()
		{
			List<RoadSection> candidates = new List<RoadSection>();
			foreach (int candidateChoiceIndex in _combinationGenerator.GetState())
			{
				if (candidateChoiceIndex == -1) break;
				candidates.Add(sectionPrototypes[candidateChoiceIndex]);
			}
			return candidates;
		}

		private TransformData _GetFirstCandidateStartPoint()
		{
			// TODO this is copied in RoadGenerator - they both define start points - should not be separate
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

		public RoadSection GetChoicePrototype()
		{
			if (!HasFoundChoice())
			{
				throw new NoChoiceFoundException();
			}
			return sectionPrototypes[_combinationGenerator.GetState()[0]];
		}
	}
}
