using System;
using System.Collections;
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
        private List<RoadSection> _currentPiecesInWorld;
        private List<RoadSection> _candidatePrototypes;

        private const int MAX_ITERATIONS = 10000000;
        private bool _impossible;

        public void Reset(List<RoadSection> currentPiecesInWorld, List<RoadSection> possibleChoicesInPreferenceOrder, int checkDepth)
        {
            _combinationGenerator = new DFSCombinationGenerator(possibleChoicesInPreferenceOrder.Count, checkDepth);
            _currentPiecesInWorld = currentPiecesInWorld;
            _candidatePrototypes = possibleChoicesInPreferenceOrder;
            _impossible = false;
        }

        public void StepUntilChoiceIsFound()
        {
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                if (_combinationGenerator.IsImpossible()) break;
                if (HasFoundChoice()) break;
                Step(1);
            }
        }

        public void Step(int choiceEngineStepsPerFrame)
        {
            if (HasFoundChoice() || _impossible) return;
            if (_combinationGenerator.IsImpossible()) return;
            if (_DoesLastCandidateSectionOverlapWithOthers())
            {
                try
                {
                    _combinationGenerator.StepInvalid();
                }
                catch (DFSCombinationGenerator.OutOfCombinationsException _)
                {
                    Debug.Log("No Choice Found!");
                    _impossible = true;
                }
            }
            else
            {
                _combinationGenerator.StepValid();
            }
        }

        private bool _DoesLastCandidateSectionOverlapWithOthers()
        {
            List<RoadSectionShape> allPiecesAligned = _GetCandidatesAndCurrentPiecesInWorldAligned();
            // TODO can allPiecesAligned.Count ever be zero? If so, prevent it from breaking
            RoadSectionShape currentCandidate = allPiecesAligned[allPiecesAligned.Count - 1];

            foreach (RoadSectionShape worldRoadSectionShape in allPiecesAligned.Take(allPiecesAligned.Count - 1))
            {
                if (currentCandidate.DoesOverlapWith(worldRoadSectionShape)) return true;
            }
            return false;
        }

        private List<RoadSectionShape> _GetCandidatesAndCurrentPiecesInWorldAligned()
        {
            return _GetCurrentPiecesInWorldShapes().Concat(_GetCandidatesAligned()).ToList();
        }

        private List<RoadSectionShape> _GetCurrentPiecesInWorldShapes()
        {
            return _currentPiecesInWorld.Select(section => section.GetShape()).ToList();
        }

        private List<RoadSectionShape> _GetCandidatesAligned()
        {
            // Figuring out the architecture so this method could exist was a nightmare.
            // Both big redesigns were a result of this.
            // I hope it looks obvious and easy to make yourself - that means I've done it right
            List<RoadSectionShape> alignedCandidates = new List<RoadSectionShape>();
            TransformData nextStartPoint = _GetFirstCandidateStartPoint();
            foreach (RoadSection candidateSection in _GetCandidatesNotAligned())
            {
                RoadSectionShape alignedCandidateShape = candidateSection.GetShape().GetTranslatedCopy(nextStartPoint);
                alignedCandidates.Add(alignedCandidateShape);
                nextStartPoint = alignedCandidateShape.End;
            }
            return alignedCandidates;
        }

        private List<RoadSection> _GetCandidatesNotAligned()
        {
            List<RoadSection> candidates = new List<RoadSection>();
            foreach (int candidateChoiceIndex in _combinationGenerator.GetState())
            {
                if (candidateChoiceIndex == -1) break;
                candidates.Add(_candidatePrototypes[candidateChoiceIndex]);
            }
            return candidates;
        }

        private TransformData _GetFirstCandidateStartPoint()
        {
            // TODO this is copied in RoadGenerator - they both define start points - should not be separate
            if (_currentPiecesInWorld.Count == 0)
            {
                return new TransformData(Vector3.zero, Quaternion.Euler(0, 0, 1), Vector3.one);
            }
            return _currentPiecesInWorld[_currentPiecesInWorld.Count - 1].GetShape().End;

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
            return _candidatePrototypes[_combinationGenerator.GetState()[0]];
        }
    }
}