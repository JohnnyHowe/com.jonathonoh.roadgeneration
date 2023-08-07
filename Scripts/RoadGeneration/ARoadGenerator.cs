using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public abstract class ARoadGenerator : MonoBehaviour
    {
        [SerializeField] private int _choiceEngineStepsPerFrame = 1;
        [SerializeField] private int _choiceEngineCheckDepth = 5;
        [SerializeField] private List<RoadSection> _roadSectionChoices; // Cannot serialize interfaces for inspector :(
        [SerializeField] private Transform _roadSectionContainer;
        [SerializeField] private bool _autoHorizontalFlipPieces = true;
        private RoadGeneratorChoiceEngine _choiceEngine;
        protected List<RoadSection> currentPieces;
        protected List<RoadSection> prototypes;

        protected void Awake()
        {
            currentPieces = new List<RoadSection>();
            _CreatePrototypes();

            _choiceEngine = new RoadGeneratorChoiceEngine();
            foreach (Transform child in _roadSectionContainer)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                RoadSection section = child.GetComponent<RoadSection>();
                // ((RoadSection)section)._SetShape();
                currentPieces.Add(section);
            }
            _ResetEngine();
        }

        private void _CreatePrototypes()
        {
            if (_autoHorizontalFlipPieces)
            {
                Debug.LogWarning("RoadSection auto flip only works when start is along z axis!");
            }

            prototypes = new List<RoadSection>();
            foreach (RoadSection roadSection in _roadSectionChoices)
            {
                prototypes.Add(_CreatePrototype(roadSection));

                RoadSection flippedSection = _CreatePrototype(roadSection);
                _Flip(flippedSection);
                flippedSection.name += "Flipped";
                prototypes.Add(flippedSection);
            }
        }

        private RoadSection _CreatePrototype(RoadSection toCopy)
        {
            GameObject prototype = Instantiate(toCopy.gameObject);
            prototype.name = toCopy.name + " Prototype";
            prototype.transform.parent = transform;
            prototype.SetActive(false);
            prototype.transform.SetGlobalScale(_roadSectionContainer.lossyScale);
            RoadSection instantiatedSection = prototype.GetComponent<RoadSection>();
            return instantiatedSection;
        }

        private void _Flip(RoadSection toFlip)
        {
            Vector3 localScale = toFlip.transform.localScale;
            localScale.x *= -1;
            toFlip.transform.localScale = localScale;
        }

        protected void Update()
        {
            _choiceEngine.Step(_choiceEngineStepsPerFrame);
            if (ShouldPlaceNewPiece())
            {
                try
                {
                    _choiceEngine.StepUntilChoiceIsFound();
                }
                catch (RoadGeneratorChoiceEngine.NoChoiceFoundException _)
                {
                    // Debug.Log("Could not find valid road section choice!");
                }
                if (_choiceEngine.HasFoundChoice())
                {
                    _PlaceNewPiece();
                    OnNewPiecePlaced(currentPieces[currentPieces.Count - 1]);
                }
                else
                {
                    // Debug.Log("Could not find valid road section choice!");
                }
            }

            if (ShouldRemoveLastPiece())
            {
                _RemoveLastPiece();
                OnPieceRemoved();
            }
        }

        protected virtual void OnNewPiecePlaced(RoadSection newPiece) { }
        protected virtual void OnPieceRemoved() { }
        protected abstract bool ShouldPlaceNewPiece();
        protected abstract bool ShouldRemoveLastPiece();
        protected abstract List<RoadSection> GetPiecesInPreferenceOrder(List<RoadSection> sectionPrototypes);

        private List<RoadSection> _GetCurrentPiecesInWorld()
        {
            return currentPieces;
        }

        private void _RemoveLastPiece()
        {
            if (currentPieces.Count == 0) return;
            RoadSection lastSection = currentPieces[0];
            currentPieces.RemoveAt(0);
            // TODO not this
            Destroy(((RoadSection)lastSection).gameObject);
            _ResetEngine();
        }

        private void _PlaceNewPiece()
        {
            RoadSection prototype = (RoadSection)_choiceEngine.GetChoicePrototype();
            RoadSection newSection = (RoadSection)prototype.Clone();
            Vector3 sectionScaleOriginal = newSection.transform.localScale;
            newSection.transform.parent = _roadSectionContainer;
            newSection.transform.SetGlobalScale(sectionScaleOriginal);
            newSection.AlignByStartPoint(_GetNextPieceStartPosition());
            currentPieces.Add(newSection);
            _ResetEngine();
        }

        private void _ResetEngine()
        {
            _choiceEngine.Reset(_GetCurrentPiecesInWorld(), GetPiecesInPreferenceOrder(prototypes).Cast<RoadSection>().ToList(), _choiceEngineCheckDepth);
        }

        private TransformData _GetNextPieceStartPosition()
        {
            if (currentPieces.Count == 0)
            {
                return new TransformData(Vector3.zero, new Quaternion(0, 0, 0, 1), Vector3.one);
            }
            return currentPieces[currentPieces.Count - 1].GetShape().End;
        }
    }
}