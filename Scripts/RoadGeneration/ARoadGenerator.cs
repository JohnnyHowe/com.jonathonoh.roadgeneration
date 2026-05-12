using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.ChoiceEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace JonathonOH.RoadGeneration
{
	public abstract class ARoadGenerator : MonoBehaviour
	{
		[SerializeField] private int _choiceEngineCheckDepth = 5;
		[SerializeField] protected List<RoadSection> _roadSectionChoices;
		[FormerlySerializedAs("_roadSectionContainer")]
		[SerializeField] public Transform roadSectionContainer;
		[SerializeField] protected RoadSectionPool roadSectionPool = new RoadSectionPool();

		private RoadGeneratorChoiceEngine choiceEngine;
		private List<RoadSection> presetPieces;

		protected virtual void OnNewPiecePlaced(RoadSection newPiece) { }
		protected virtual void OnPieceRemoved() { }
		protected abstract bool ShouldPlaceNewPiece();
		protected abstract bool ShouldRemoveLastPiece();
		protected virtual void OnNoChoiceFound()
		{
			Debug.LogError("No RoadSection choice found!");
		}

		protected void Awake()
		{
			roadSectionPool.Reset(_roadSectionChoices, roadSectionContainer);
			PopulateCurrentSectionsFromWorld();
		}

		protected void Start()
		{
			ResetEngine();
		}

		private void PopulateCurrentSectionsFromWorld()
		{
			presetPieces = new List<RoadSection>();
			foreach (Transform child in roadSectionContainer)
			{
				if (!child.gameObject.activeInHierarchy) continue;

				RoadSection section = child.GetComponent<RoadSection>();
				presetPieces.Add(section);

				section.N = presetPieces.Count - 1;
			}
		}

		protected void Update()
		{
			choiceEngine.Step();
			if (ShouldPlaceNewPiece())
			{
				try
				{
					choiceEngine.StepUntilChoiceIsFound();
				}
				catch (RoadGeneratorChoiceEngine.NoChoiceFoundException)
				{
					OnNoChoiceFound();
				}
				if (choiceEngine.HasFoundChoice())
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
			}
		}
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

			OnPieceRemoved();
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

			RoadSection roadSection = roadSectionPool.ClaimUninstantiatedSection(choiceEngine.GetChoicePrototype());
			roadSection.N = nextN;
			roadSection.AlignByStartPoint(nextStartPosition);
			roadSectionPool.ActivateSection(roadSection);
			ResetEngine();

			return roadSection;
		}

		private void ResetEngine()
		{
			List<RoadSection> choices = GetPiecesInPreferenceOrder(roadSectionPool.GetAllAvailablePrototypes().ToList());

			ChoiceRequest choiceRequest = new ChoiceRequest()
			{
				CurrentSectionsInWorld = GetAllCurrentSections().ToList(),
				SectionsInPreferenceOrder = choices,
				MaxCheckDepth = _choiceEngineCheckDepth
			};

			if (choices.Count == 0)
			{
				OnPoolEmpty();
			}
			else {
				choiceEngine = new RoadGeneratorChoiceEngine(choiceRequest);
			}
		}

		public RoadSection GetNewestSection()
		{
			RoadSection newestSection = roadSectionPool.GetNewestSection();
			if (newestSection) return newestSection;

			if (presetPieces.Count != 0) return presetPieces[presetPieces.Count - 1];

			return null;
		}

		public RoadSection GetOldestSection()
		{
			if (presetPieces.Count != 0) return presetPieces[0];
			return roadSectionPool.GetOldestSection();
		}

		public IEnumerable<RoadSection> GetAllCurrentSections()
		{
			foreach (RoadSection section in presetPieces) { yield return section; }
			foreach (RoadSection section in roadSectionPool.GetAllUsedSectionsOrdered()) { yield return section; }
		}
	}
}
