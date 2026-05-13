using System.Collections;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullUtilityTests
{
	internal sealed class Vector2EqualityComparer : IEqualityComparer
	{
		public static readonly Vector2EqualityComparer Instance = new Vector2EqualityComparer();

		public new bool Equals(object x, object y)
		{
			if (x is not Vector2 left || y is not Vector2 right)
			{
				return false;
			}

			return Vector2.Distance(left, right) < 0.0001f;
		}

		public int GetHashCode(object obj)
		{
			return obj?.GetHashCode() ?? 0;
		}
	}
}
