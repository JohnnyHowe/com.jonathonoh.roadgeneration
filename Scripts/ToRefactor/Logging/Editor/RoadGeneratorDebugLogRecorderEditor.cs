using UnityEditor;
using UnityEngine;

namespace JonathonOH.RoadGeneration.Logging.Editor
{
	[CustomEditor(typeof(RoadGeneratorDebugLogRecorder))]
	public sealed class RoadGeneratorDebugLogRecorderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Log Window"))
			{
				RoadGenerationLoggingWindow.ShowWindow();
			}

			DrawDefaultInspector();
		}
	}
}
