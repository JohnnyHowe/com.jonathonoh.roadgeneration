using Other;
using UnityEngine;
using UnityEngine.Events;

namespace JonathonOH.RoadGeneration.Logging
{
	[RequireComponent(typeof(ARoadGenerator))]
	public class RoadGeneratorDebugLogRecorder : MonoBehaviour
	{
		[SerializeField] private bool logToConsole;
		[SerializeField][ReadOnly] private RoadGeneratorDebugLog log;

		private ARoadGenerator roadGenerator;
		private UnityAction<RoadSection> onNewSectionPlacedValue;
		private UnityAction onNewSectionPlaced;
		private UnityAction onLastSectionRemoved;
		private UnityAction onNoChoiceFound;
		private UnityAction onPoolEmpty;

		public RoadGeneratorDebugLog Log => log;

		private void Awake()
		{
			log = new RoadGeneratorDebugLog();

			roadGenerator = GetComponent<ARoadGenerator>();

			onNewSectionPlacedValue = roadSection => AddLog($"NewSectionPlacedValue invoked with {roadSection}");
			onNewSectionPlaced = () => AddLog("NewSectionPlaced invoked");
			onLastSectionRemoved = () => AddLog("LastSectionRemoved invoked");
			onNoChoiceFound = () => AddLog("NoChoiceFound invoked");
			onPoolEmpty = () => AddLog("PoolEmpty invoked");
		}

		private void AddLog(string text)
		{
			if (logToConsole)
			{
				Debug.Log(text);
			}
			log.Add(text);
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
