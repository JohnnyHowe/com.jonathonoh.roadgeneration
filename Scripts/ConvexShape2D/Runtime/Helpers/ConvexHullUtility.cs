using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	internal static class ConvexHullUtility
	{
		public static List<Vector2> GetTangentsFromHull(IReadOnlyList<Vector2> convexHullClockwise)
		{
			List<Vector2> tangents = new List<Vector2>();
			foreach ((Vector2, Vector2) edge in GetEdges(convexHullClockwise))
			{
				tangents.Add((edge.Item2 - edge.Item1).normalized);
			}
			return tangents.Distinct(TangentComparer.Instance).ToList();
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
