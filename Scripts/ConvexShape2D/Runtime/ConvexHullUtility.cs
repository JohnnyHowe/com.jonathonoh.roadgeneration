using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public static class ConvexHullUtility
	{
		private class Vector2CloseEnoughEqualityComparer : IEqualityComparer<Vector2>
		{
			public bool Equals(Vector2 v1, Vector2 v2)
			{
				return (v1 - v2).SqrMagnitude() < Mathf.Epsilon;
			}

			public int GetHashCode(Vector2 vector)
			{
				return vector.GetHashCode();
			}
		}

		private class TangentCloseEnoughEqualityComparer : IEqualityComparer<Vector2>
		{
			private static Vector2CloseEnoughEqualityComparer comparer = new Vector2CloseEnoughEqualityComparer();
			public bool Equals(Vector2 v1, Vector2 v2)
			{
				return (v1 - v2).SqrMagnitude() < Mathf.Epsilon || (v1 + v2).SqrMagnitude() < Mathf.Epsilon;
			}

			public int GetHashCode(Vector2 vector)
			{
				return vector.GetHashCode();
			}
		}

		private static IEqualityComparer<Vector2> tangentComparer = new TangentCloseEnoughEqualityComparer();

		public static List<Vector2> GetTangentsFromHull(IReadOnlyList<Vector2> convexHullClockwise)
		{
			List<Vector2> tangents = new List<Vector2>();
			foreach ((Vector2, Vector2) edge in GetEdges(convexHullClockwise))
			{
				tangents.Add((edge.Item2 - edge.Item1).normalized);
			}
			return tangents.Distinct(tangentComparer).ToList();
		}

		internal static IEnumerable<(Vector2, Vector2)> GetEdges(IReadOnlyList<Vector2> convexHullClockwise)
		{
			int nPoints = convexHullClockwise.Count;
			for (int i = 0; i < nPoints; i++)
			{
				Vector2 current = convexHullClockwise[i];
				Vector2 next = convexHullClockwise[(i + 1) % nPoints];
				if ((current - next).SqrMagnitude() > Mathf.Epsilon)
				{
					yield return (current, next);
				}
			}
		}

		public static IEnumerable<Vector2> GetNormalsFromTangents(IEnumerable<Vector2> tangents)
		{
			foreach (Vector2 tangent in tangents)
			{
				yield return new Vector2(-tangent.y, tangent.x);
			}
		}
	}
}
