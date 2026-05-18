
using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.Geometry
{
	internal static class BoundingRectCalculator
	{
		internal static Rect FromVertices(IReadOnlyList<Vector2> Vertices)
		{
			if (Vertices.Count == 0)
				return new Rect();

			Vector2 min = Vertices[0];
			Vector2 max = Vertices[0];

			for (int i = 1; i < Vertices.Count; i++)
			{
				min = Vector2.Min(min, Vertices[i]);
				max = Vector2.Max(max, Vertices[i]);
			}

			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}
	}
}
