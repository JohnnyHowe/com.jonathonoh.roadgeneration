
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public static class ProjectionUtility
	{
		public static FloatRange ProjectAll(IReadOnlyList<Vector2> points, Vector2 axis)
		{
			if (points.Count == 0)
			{
				throw new ArgumentException("Cannot project zero points!");
			}

			float min = Mathf.Infinity;
			float max = -Mathf.Infinity;
			foreach (Vector2 vertex in points)
			{
				float vertexProjection = Project(vertex, axis);
				min = Mathf.Min(min, vertexProjection);
				max = Mathf.Max(max, vertexProjection);
			}
			return new FloatRange(min, max);
		}

		public static float Project(Vector2 point, Vector2 axis)
		{
			return Vector2.Dot(axis, point) / axis.magnitude;
		}
	}
}
