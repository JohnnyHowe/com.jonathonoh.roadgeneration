using System;
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.Logging
{
	[Serializable]
	public class RoadGeneratorDebugLogEntry
	{
		public string Log;
		public List<DateTime> Timestamps = new List<DateTime>();
	}
}
