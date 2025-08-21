using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public abstract class ARoadGenerator : MonoBehaviour
    {
        [SerializeField] private int _choiceEngineCheckDepth = 5;
        [SerializeField] private int minObjectsPerSectionForPool = 5;
        [SerializeField] protected List<RoadSection> _roadSectionChoices;
        [SerializeField] private Transform _roadSectionContainer;

        protected RoadSectionPool roadSectionPool;
        private RoadGeneratorChoiceEngine _choiceEngine;
        private List<RoadSection> presetPieces;

        protected void Awake()
        {
            roadSectionPool = new RoadSectionPool(_roadSectionChoices, _roadSectionContainer, minObjectsPerSectionForPool);
            _choiceEngine = new RoadGeneratorChoiceEngine();
            PopulateCurrentPiecesFromWorld();
            ResetEngine();
        }

        private void PopulateCurrentPiecesFromWorld()
        {
            presetPieces = new List<RoadSection>();
            foreach (Transform child in _roadSectionContainer)
            {
                if (!child.gameObject.activeInHierarchy) continue;

                RoadSection section = child.GetComponent<RoadSection>();
                presetPieces.Add(section);

                section.N = presetPieces.Count - 1;
            }
        }

        protected void Update()
        {
            _choiceEngine.Step();
            if (ShouldPlaceNewPiece())
            {
                try
                {
                    _choiceEngine.StepUntilChoiceIsFound();
                }
                catch (RoadGeneratorChoiceEngine.NoChoiceFoundException)
                {
                    OnNoChoiceFound();
                }
                if (_choiceEngine.HasFoundChoice())
                {
                    RoadSection newPiece = TryPlaceNewPiece();
                    if (newPiece is null) OnPoolEmpty();
                    else OnNewPiecePlaced(newPiece);
                }
                else
                {
                    OnNoChoiceFound();
                }
            }

            if (ShouldRemoveLastPiece())
            {
                RemoveLastPiece();
                OnPieceRemoved();
            }
        }

        protected virtual void OnNewPiecePlaced(RoadSection newPiece) { }
        protected virtual void OnPieceRemoved() { }
        protected abstract bool ShouldPlaceNewPiece();
        protected abstract bool ShouldRemoveLastPiece();
        protected virtual void OnNoChoiceFound() { }
        protected virtual void OnPoolEmpty() { }
        protected abstract List<RoadSection> GetPiecesInPreferenceOrder(List<RoadSection> sectionPrototypes);

        protected void RemoveLastPiece()
        {
            if (presetPieces.Count > 0)
            {
                Destroy(presetPieces[0].gameObject);
                presetPieces.RemoveAt(0);
            }
            else
            {
                roadSectionPool.ReleaseOldestInstantiatedSection();
            }
            ResetEngine();
        }

        private RoadSection TryPlaceNewPiece()
        {
            if (roadSectionPool.GetAllAvailablePrototypes().Count() == 0) return null;

            int nextN = 0;
            RoadSection newestSection = GetNewestSection();
            TransformData nextStartPosition = new TransformData(Vector3.zero, new Quaternion(0, 0, 0, 1), Vector3.one);
            if (newestSection != null)
            {
                nextN = newestSection.N + 1;
                nextStartPosition = newestSection.GetShape().End;
            }

            RoadSection roadSection = roadSectionPool.ClaimUninstantiatedSection(_choiceEngine.GetChoicePrototype());
            roadSection.N = nextN;

            roadSection.AlignByStartPoint(nextStartPosition);
            ResetEngine();

            return roadSection;
        }

        private void ResetEngine()
        {
            List<RoadSection> choices = GetPiecesInPreferenceOrder(roadSectionPool.GetAllAvailablePrototypes().ToList());
            if (choices.Count == 0) OnPoolEmpty();
            else _choiceEngine.Reset(GetAllCurrentSections().ToList(), choices, _choiceEngineCheckDepth);
        }

        private RoadSection GetNewestSection()
        {
            RoadSection newestSection = roadSectionPool.GetNewestSection();
            if (newestSection) return newestSection;

            if (presetPieces.Count != 0) return presetPieces[presetPieces.Count - 1];

            return null;
        }

        public IEnumerable<RoadSection> GetAllCurrentSections()
        {
            foreach (RoadSection section in presetPieces) { yield return section; }
            foreach (RoadSection section in roadSectionPool.GetAllUsedSectionsOrdered()) { yield return section; }
        }
    }
}