using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullUtilityTests
{
	internal sealed class Vector2EqualityComparer : IEqualityComparer<Vector2>
	{
		public static readonly Vector2EqualityComparer Instance = new Vector2EqualityComparer();

		public bool Equals(Vector2 x, Vector2 y)
		{
			return Vector2.Distance(x, y) < 0.0001f
				|| Vector2.Distance(x, -y) < 0.0001f;
		}

		public int GetHashCode(Vector2 obj)
		{
			return obj.GetHashCode();
		}
	}
}
