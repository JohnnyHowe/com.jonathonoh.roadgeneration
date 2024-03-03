using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public abstract class ARoadGenerator : MonoBehaviour
    {
        [Serializable]
        private struct PresetPiece
        {
            public bool flipped;
            public int pieceIndex;
        }
        [SerializeField] private int _choiceEngineStepsPerFrame = 1;
        [SerializeField] private int _choiceEngineCheckDepth = 5;
        [SerializeField] protected List<RoadSection> _roadSectionChoices;
        [SerializeField] private Transform _roadSectionContainer;
        [SerializeField] private bool _autoHorizontalFlipPieces = true;
        [SerializeField] private bool placeAllPresetPiecesOnStart = true;
        [SerializeField] private List<PresetPiece> piecesToPlaceFirst;
        private RoadGeneratorChoiceEngine _choiceEngine;
        protected List<RoadSection> currentPieces;
        protected List<RoadSection> prototypes;
        private int presetPiecesPlaced;
        private bool AllPresetPiecesPlaced
        {
            get => presetPiecesPlaced == piecesToPlaceFirst.Count;
        }

        protected void Awake()
        {
            currentPieces = new List<RoadSection>();
            presetPiecesPlaced = 0;
            _CreatePrototypes();
            _choiceEngine = new RoadGeneratorChoiceEngine();
            PopulateCurrentPiecesFromWorld();
            if (placeAllPresetPiecesOnStart) PlaceAllPresetPieces();
            _ResetEngine();
        }

        private void PopulateCurrentPiecesFromWorld()
        {
            foreach (Transform child in _roadSectionContainer)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                RoadSection section = child.GetComponent<RoadSection>();
                currentPieces.Add(section);
            }
        }

        private void PlaceAllPresetPieces()
        {
            while (!AllPresetPiecesPlaced) PlaceNextPresetPiece();
        }

        private void PlaceNextPresetPiece()
        {
            PlacePresetPiece(piecesToPlaceFirst[presetPiecesPlaced++]);
        }

        private void PlacePresetPiece(PresetPiece presetPiece)
        {
            int prototypeIndex = presetPiece.pieceIndex;
            if (_autoHorizontalFlipPieces) prototypeIndex = prototypeIndex * 2 + (presetPiece.flipped ? 1 : 0);
            _PlaceNewPiece(prototypes[prototypeIndex]);
        }

        private void _CreatePrototypes()
        {
            if (_autoHorizontalFlipPieces)
            {
                // Debug.LogWarning("RoadSection auto flip only works when start is along z axis!");
            }

            int i = 0;
            prototypes = new List<RoadSection>();
            foreach (RoadSection roadSection in _roadSectionChoices)
            {
                RoadSection section = _CreatePrototype(roadSection);
                section.PieceTypeId = i;
                prototypes.Add(section);

                RoadSection flippedSection = _CreatePrototype(roadSection);
                flippedSection.PieceTypeId = i;
                _Flip(flippedSection);
                flippedSection.IsFlipped = true;
                flippedSection.name += "Flipped";
                prototypes.Add(flippedSection);

                i++;
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
            if (AllPresetPiecesPlaced)
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
            }
            else if (ShouldPlaceNewPiece())
            {
                PlaceNextPresetPiece();
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
            _PlaceNewPiece((RoadSection)_choiceEngine.GetChoicePrototype());
        }

        private void _PlaceNewPiece(RoadSection prototype)
        {
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