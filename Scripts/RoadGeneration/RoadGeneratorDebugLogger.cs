using UnityEngine;
using UnityEngine.Events;

namespace JonathonOH.RoadGeneration
{
	/// <summary>
	/// Purely logging for the ARoadGenerator.
	/// </summary>
	[RequireComponent(typeof(ARoadGenerator))]
	public class RoadGeneratorDebugLogger : MonoBehaviour
	{
		private ARoadGenerator roadGenerator;

		private UnityAction<RoadSection> onNewSectionPlacedValue;
		private UnityAction onNewSectionPlaced;
		private UnityAction onLastSectionRemoved;
		private UnityAction onNoChoiceFound;
		private UnityAction onPoolEmpty;

		private bool verbose => enabled && gameObject.activeInHierarchy;

		private void Awake()
		{
			roadGenerator = GetComponent<ARoadGenerator>();

			onNewSectionPlacedValue = roadSection =>
			{
				if (!verbose) return;
				Debug.Log($"NewSectionPlacedValue invoked with {roadSection}", this);
			};

			onNewSectionPlaced = () =>
			{
				if (!verbose) return;
				Debug.Log("NewSectionPlaced invoked", this);
			};

			onLastSectionRemoved = () =>
			{
				if (!verbose) return;
				Debug.Log("LastSectionRemoved invoked", this);
			};

			onNoChoiceFound = () =>
			{
				if (!verbose) return;
				Debug.Log("NoChoiceFound invoked", this);
			};

			onPoolEmpty = () =>
			{
				if (!verbose) return;
				Debug.Log("PoolEmpty invoked", this);
			};
		}

		private void OnEnable()
		{
			roadGenerator.NewSectionPlacedValue.AddListener(onNewSectionPlacedValue);
			roadGenerator.NewSectionPlaced.AddListener(onNewSectionPlaced);
			roadGenerator.LastSectionRemoved.AddListener(onLastSectionRemoved);
			roadGenerator.NoChoiceFound.AddListener(onNoChoiceFound);
			roadGenerator.PoolEmpty.AddListener(onPoolEmpty);
		}

		private void OnDisable()
		{
			roadGenerator.NewSectionPlacedValue.RemoveListener(onNewSectionPlacedValue);
			roadGenerator.NewSectionPlaced.RemoveListener(onNewSectionPlaced);
			roadGenerator.LastSectionRemoved.RemoveListener(onLastSectionRemoved);
			roadGenerator.NoChoiceFound.RemoveListener(onNoChoiceFound);
			roadGenerator.PoolEmpty.RemoveListener(onPoolEmpty);
		}
	}
}
