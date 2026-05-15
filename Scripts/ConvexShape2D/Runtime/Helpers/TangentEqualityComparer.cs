
using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	internal class TangentComparer : IEqualityComparer<Vector2>
	{
		public static TangentComparer Instance { get; private set; } = new TangentComparer();

		public bool Equals(Vector2 v1, Vector2 v2)
		{
			return AreVectorsSimilarEnough(v1, v2)
				|| AreVectorsSimilarEnough(v1, -v2);
		}

		private static bool AreVectorsSimilarEnough(Vector2 v1, Vector2 v2)
		{
			return (v1 - v2).normalized == Vector2.zero;
		}

		public int GetHashCode(Vector2 vector)
		{
			return 0;
		}
	}
}
