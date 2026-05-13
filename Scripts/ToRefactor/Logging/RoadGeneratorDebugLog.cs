using System;
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.Logging
{
	[Serializable]
	public class RoadGeneratorDebugLog
	{
		public List<RoadGeneratorDebugLogEntry> Entries = new List<RoadGeneratorDebugLogEntry>();

		public void Add(string log)
		{
			DateTime now = DateTime.Now;

			if (Entries.Count > 0 && Entries[Entries.Count - 1].Log == log)
			{
				Entries[Entries.Count - 1].Timestamps.Add(now);
				return;
			}

			Entries.Add(new RoadGeneratorDebugLogEntry()
			{
				Log = log,
				Timestamps = new List<DateTime>() { now }
			});
		}
	}
}
