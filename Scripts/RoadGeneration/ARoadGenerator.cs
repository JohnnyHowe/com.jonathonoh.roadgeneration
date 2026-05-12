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
		private List<RoadSection> presetSections;

		protected abstract bool ShouldPlaceNewSection();
		protected abstract List<RoadSection> GetSectionsInPreferenceOrder(List<RoadSection> sectionPrototypes);
		protected virtual void NewSectionPlaced(RoadSection newPiece) { }
		protected abstract bool ShouldRemoveLastSection();
		protected virtual void LastSectionRemoved() { }
		protected virtual void NoChoiceFound() { }
		protected virtual void PoolEmpty() { }

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
			presetSections = new List<RoadSection>();
			foreach (Transform child in roadSectionContainer)
			{
				if (!child.gameObject.activeInHierarchy) continue;

				RoadSection section = child.GetComponent<RoadSection>();
				presetSections.Add(section);

				section.N = presetSections.Count - 1;
			}
		}

		protected void Update()
		{
			choiceEngine.Step();

			if (ShouldPlaceNewSection())
			{
				try
				{
					choiceEngine.StepUntilChoiceIsFound();
				}
				catch (RoadGeneratorChoiceEngine.NoChoiceFoundException)
				{
					NoChoiceFound();
				}
				if (choiceEngine.HasFoundChoice())
				{
					RoadSection newPiece = TryPlaceNewPiece();
					if (newPiece is null) PoolEmpty();
					else NewSectionPlaced(newPiece);
				}
				else
				{
					NoChoiceFound();
				}
			}

			if (ShouldRemoveLastSection())
			{
				RemoveLastPiece();
			}
		}

		protected void RemoveLastPiece()
		{
			if (presetSections.Count > 0)
			{
				Destroy(presetSections[0].gameObject);
				presetSections.RemoveAt(0);
			}
			else
			{
				roadSectionPool.ReleaseOldestInstantiatedSection();
			}

			LastSectionRemoved();
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
			List<RoadSection> choices = GetSectionsInPreferenceOrder(roadSectionPool.GetAllAvailablePrototypes().ToList());

			ChoiceRequest choiceRequest = new ChoiceRequest()
			{
				CurrentSectionsInWorld = GetAllCurrentSections().ToList(),
				SectionsInPreferenceOrder = choices,
				MaxCheckDepth = _choiceEngineCheckDepth
			};

			if (choices.Count == 0)
			{
				PoolEmpty();
			}
			else
			{
				choiceEngine = new RoadGeneratorChoiceEngine(choiceRequest);
			}
		}

		public RoadSection GetNewestSection()
		{
			RoadSection newestSection = roadSectionPool.GetNewestSection();
			if (newestSection) return newestSection;

			if (presetSections.Count != 0) return presetSections[presetSections.Count - 1];

			return null;
		}

		public RoadSection GetOldestSection()
		{
			if (presetSections.Count != 0) return presetSections[0];
			return roadSectionPool.GetOldestSection();
		}

		public IEnumerable<RoadSection> GetAllCurrentSections()
		{
			foreach (RoadSection section in presetSections) { yield return section; }
			foreach (RoadSection section in roadSectionPool.GetAllUsedSectionsOrdered()) { yield return section; }
		}
	}
}
