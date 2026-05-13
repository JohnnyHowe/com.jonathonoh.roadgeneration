using UnityEngine;

namespace JonathonOH.RoadGeneration
{
	[RequireComponent(typeof(ARoadGenerator))]
	public class RoadGeneratorEventLogger : MonoBehaviour
	{
		private ARoadGenerator roadGenerator;

		private void Awake()
		{
			roadGenerator = GetComponent<ARoadGenerator>();

			roadGenerator.NewSectionPlacedValue.AddListener(roadSection => Debug.Log($"NewSectionPlacedValue invoked with {roadSection}"));
			roadGenerator.NewSectionPlaced.AddListener(() => Debug.Log("NewSectionPlaced invoked"));
			roadGenerator.LastSectionRemoved.AddListener(() => Debug.Log("LastSectionRemoved invoked"));
			roadGenerator.NoChoiceFound.AddListener(() => Debug.LogError("NoChoiceFound invoked"));
			roadGenerator.PoolEmpty.AddListener(() => Debug.LogError("PoolEmpty invoked"));
		}
	}
}
