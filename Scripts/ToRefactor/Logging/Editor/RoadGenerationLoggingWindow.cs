using UnityEditor;
using UnityEngine;

namespace JonathonOH.RoadGeneration.Logging.Editor
{
	public sealed class RoadGenerationLoggingWindow : EditorWindow
	{
		private ARoadGenerator roadGenerator;
		private RoadGeneratorDebugLogRecorder logRecorder;
		[SerializeField] private RoadGeneratorDebugLog mostRecentLog;
		[SerializeField] private Vector2 logScroll = Vector2.zero;


		[MenuItem("Window/Road Generation/Logging")]
		public static void ShowWindow()
		{
			var window = GetWindow<RoadGenerationLoggingWindow>();
			window.titleContent = new GUIContent("Road Logging");
		}

		private void OnGUI()
		{
			UpdateSceneReferences();
			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUILayout.ObjectField("Road Generator", roadGenerator, typeof(ARoadGenerator), true);
				EditorGUILayout.ObjectField("Log Recorder", logRecorder, typeof(RoadGeneratorDebugLogRecorder), true);
			}

			UpdateMostRecentLog();
			DrawStatusHeader();

			DrawScrollableLogs();
		}

		private void Update()
		{
			Repaint();
		}

		private void UpdateMostRecentLog()
		{
			if (logRecorder != null && logRecorder.Log != null && logRecorder.Log.Entries.Count > 0)
			{
				mostRecentLog = logRecorder.Log;
			}
		}

		private void DrawStatusHeader()
		{

			if (logRecorder == null && mostRecentLog == null)
			{
				EditorGUILayout.LabelField("No logger or logs to show!");
			}

			else if (logRecorder == null)
			{
				EditorGUILayout.LabelField("No logger! Showing logs from previous run.");
			}

			else if (mostRecentLog == null || mostRecentLog.Entries.Count == 0)
			{
				EditorGUILayout.LabelField("Logger found, but no logs");
			}
			else
			{
				EditorGUILayout.LabelField("We're live!");
			}
		}

		private void UpdateSceneReferences()
		{
			roadGenerator = FindFirstRoadGeneratorInScene();
			if (roadGenerator)
			{
				logRecorder = roadGenerator.gameObject.GetComponent<RoadGeneratorDebugLogRecorder>();
			}
		}

		private static ARoadGenerator FindFirstRoadGeneratorInScene()
		{
			ARoadGenerator[] roadGenerators = Resources.FindObjectsOfTypeAll<ARoadGenerator>();
			foreach (ARoadGenerator roadGenerator in roadGenerators)
			{
				if (roadGenerator == null) continue;
				if (EditorUtility.IsPersistent(roadGenerator)) continue;
				if (!roadGenerator.gameObject.scene.IsValid()) continue;

				return roadGenerator;
			}

			return null;
		}

		private void DrawScrollableLogs()
		{
			logScroll = EditorGUILayout.BeginScrollView(logScroll);
			DrawLog(mostRecentLog);
			EditorGUILayout.EndScrollView();
		}

		/// <summary>
		/// Draws the log as a table.
		/// Columns are the first timestamp, n entries, log text
		/// Does NOT handle scrolling.
		/// </summary>
		private void DrawLog(RoadGeneratorDebugLog log)
		{
			if (log == null || log.Entries == null || log.Entries.Count == 0)
			{
				return;
			}

			EditorGUILayout.Space();

			using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				EditorGUILayout.LabelField("First Timestamp", EditorStyles.miniBoldLabel, GUILayout.Width(160f));
				EditorGUILayout.LabelField("Count", EditorStyles.miniBoldLabel, GUILayout.Width(50f));
				EditorGUILayout.LabelField("Log", EditorStyles.miniBoldLabel);
			}

			foreach (RoadGeneratorDebugLogEntry entry in log.Entries)
			{
				if (entry == null)
				{
					continue;
				}

				string firstTimestamp = "-";
				int count = 0;

				if (entry.Timestamps != null && entry.Timestamps.Count > 0)
				{
					firstTimestamp = entry.Timestamps[0].ToString("HH:mm:ss.fff");
					count = entry.Timestamps.Count;
				}

				using (new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.SelectableLabel(firstTimestamp, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(160f));
					EditorGUILayout.SelectableLabel(count.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(50f));
					EditorGUILayout.SelectableLabel(entry.Log ?? string.Empty, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
				}
			}
		}
	}
}
