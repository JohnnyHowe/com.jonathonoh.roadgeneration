using System.Collections.Generic;
using JonathonOH.Geometry;
using UnityEngine;

namespace JonathonOH.ConvexShapeExtruded
{
	public readonly struct ConvexHullExtruded
	{
		public readonly ConvexPolygon HorizontalHull;
		public readonly FloatRange VerticalRange;

		public ConvexHullExtruded(ConvexPolygon horizontalHull, FloatRange verticalRange)
		{
			HorizontalHull = horizontalHull;
			VerticalRange = verticalRange;
		}

		public static ConvexHullExtruded FromMesh(Mesh mesh, bool infiniteHeight)
		{
			return FromVertices(mesh.vertices, infiniteHeight);
		}

		public static ConvexHullExtruded FromVertices(IEnumerable<Vector3> vertices, bool infiniteHeight)
		{
			List<Vector2> horizontalVertices = new List<Vector2>();

			float maxVerticalPosition = -Mathf.Infinity;
			float minVerticalPosition = Mathf.Infinity;

			foreach (Vector3 vertex in vertices)
			{
				horizontalVertices.Add(new Vector2(vertex.x, vertex.z));
				if (!infiniteHeight)
				{
					maxVerticalPosition = Mathf.Max(maxVerticalPosition, vertex.y);
					minVerticalPosition = Mathf.Min(minVerticalPosition, vertex.y);
				}
			}

			FloatRange verticalRange = new FloatRange(minVerticalPosition, maxVerticalPosition);
			ConvexPolygon horizontalHull = new ConvexPolygon(horizontalVertices);
			return new ConvexHullExtruded(horizontalHull, verticalRange);
		}

		public bool OverlapsWith(ConvexHullExtruded other)
		{
			if (!VerticalRange.OverlapsWith(other.VerticalRange))
			{
				return false;
			}
			return HorizontalHull.OverlapsWith(other.HorizontalHull);
		}
	}
}
