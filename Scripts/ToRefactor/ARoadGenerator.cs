using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.ChoiceEngine;
using JonathonOH.RoadGeneration.Collision;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JonathonOH.RoadGeneration
{
	public abstract class ARoadGenerator : MonoBehaviour
	{
		public readonly UnityEvent<RoadSection> NewSectionPlacedValue = new UnityEvent<RoadSection>();
		public readonly UnityEvent NewSectionPlaced = new UnityEvent();
		public readonly UnityEvent LastSectionRemoved = new UnityEvent();
		public readonly UnityEvent NoChoiceFound = new UnityEvent();
		public readonly UnityEvent PoolEmpty = new UnityEvent();

		[SerializeField] private int _choiceEngineCheckDepth = 5;
		[SerializeField] protected List<RoadSection> _roadSectionChoices;
		[FormerlySerializedAs("_roadSectionContainer")]
		[SerializeField] public Transform roadSectionContainer;
		[SerializeField] protected RoadSectionPool roadSectionPool = new RoadSectionPool();

		private RoadGeneratorChoiceEngine choiceEngine;
		private List<RoadSection> presetSections;

		protected abstract bool ShouldPlaceNewSection();
		protected abstract List<RoadSection> GetSectionsInPreferenceOrder(List<RoadSection> sectionPrototypes);
		protected abstract bool ShouldRemoveLastSection();

		protected void Awake()
		{
			NewSectionPlacedValue.AddListener((roadSection) => NewSectionPlaced.Invoke());
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
				TryPlaceNewSection();
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

			LastSectionRemoved.Invoke();
			ResetEngine();
		}

		private void TryPlaceNewSection()
		{
			choiceEngine.StepUntilChoiceIsFound();

			var result = choiceEngine.CurrentChoiceResult;
			if (!result.IsChoiceFound)
			{
				NoChoiceFound.Invoke();
				return;
			}

			RoadSection newSection = TryPlaceNewSection(result.ChosenSection);
			if (newSection is null)
			{
				PoolEmpty.Invoke();
			}
			else
			{
				NewSectionPlacedValue.Invoke(newSection);
			}
		}

		private RoadSection TryPlaceNewSection(RoadSection prototype)
		{
			if (roadSectionPool.GetAllAvailablePrototypes().Count() == 0) return null;

			int nextN = 0;
			RoadSection newestSection = GetNewestSection();
			TransformData nextStartPosition = new TransformData(Vector3.zero, new Quaternion(0, 0, 0, 1), Vector3.one);
			if (newestSection != null)
			{
				nextN = newestSection.N + 1;
				nextStartPosition = newestSection.EndPoint;
			}

			RoadSection roadSection = roadSectionPool.ClaimUninstantiatedSection(prototype);
			roadSection.N = nextN;
			roadSection.AlignStart(nextStartPosition);
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
				PoolEmpty.Invoke();
			}
			else
			{
				choiceEngine = new RoadGeneratorChoiceEngine(choiceRequest, new RoadSectionShapeCollisionChecker());
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
