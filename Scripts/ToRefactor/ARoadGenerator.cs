using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Core;
using JonathonOH.RoadGeneration.RoadSectionShapeCollision;
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

		[SerializeField] public int _choiceEngineCheckDepth = 5;
		[SerializeField] private int stepsPerFrame = 4;
		[SerializeField] public List<RoadSection> _roadSectionChoices;
		[FormerlySerializedAs("_roadSectionContainer")]
		[SerializeField] public Transform roadSectionContainer;
		[SerializeField] protected RoadSectionPool roadSectionPool = new RoadSectionPool();
		[SerializeField] private bool allowSteppingUntilResultFound = true;

		private RoadGeneratorChoiceEngine choiceEngine;
		private List<RoadSection> presetSections;

		protected abstract bool ShouldPlaceNewSection();
		protected abstract List<RoadSection> GetSectionsInPreferenceOrder(List<RoadSection> sectionPrototypes);
		protected abstract bool ShouldRemoveLastSection();

		protected void Awake()
		{
			choiceEngine = new RoadGeneratorChoiceEngine(new RoadSectionShapeCollisionEngine());
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
			StepEngine();

			if (ShouldPlaceNewSection())
			{
				TryPlaceNewSection();
			}
			if (ShouldRemoveLastSection())
			{
				RemoveLastPiece();
			}
		}

		private void StepEngine()
		{
			for (int i = 0; i < Mathf.Max(1, stepsPerFrame); i++)
			{
				if (!choiceEngine.IsSearching())
				{
					break;
				}
				choiceEngine.Step();
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
			if (!choiceEngine.IsSearchFinished() && allowSteppingUntilResultFound)
			{
				choiceEngine.StepUntilChoiceFound();
			}

			if (choiceEngine.IsSearchFinished())
			{
				ChoiceResult result = (ChoiceResult)choiceEngine.CurrentChoiceResult;
				TryPlaceNewSection(result);
			}
		}

		private void TryPlaceNewSection(ChoiceResult choiceResult)
		{
			if (!choiceResult.IsChoiceFound)
			{
				NoChoiceFound.Invoke();
			}
			else
			{
				TryPlaceNewSection(choiceResult.ChosenSection);
			}
		}

		private void TryPlaceNewSection(IRoadSection chosenSectionPrototypeI)
		{
			// TODO not this
			// This is temporary!!!!
			RoadSection chosenSectionPrototype = (RoadSection)chosenSectionPrototypeI;

			if (roadSectionPool.GetAllAvailablePrototypes().Count() == 0)
			{
				// TODO should this add more to the pool?
				// Or do we filter preference list by what's in the pool?
				PoolEmpty.Invoke();
				return;
			}

			int nextN = 0;
			RoadSection newestSection = GetNewestSection();
			Pose nextStart = Pose.identity;
			if (newestSection != null)
			{
				nextN = newestSection.N + 1;
				nextStart = newestSection.Exit;
			}

			RoadSection roadSection = roadSectionPool.ClaimUninstantiatedSection(chosenSectionPrototype);
			roadSection.N = nextN;
			roadSection.AlignByEntry(nextStart);
			roadSectionPool.ActivateSection(roadSection);
			ResetEngine();

			NewSectionPlacedValue.Invoke(roadSection);
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
				choiceEngine.Reset(choiceRequest);
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
