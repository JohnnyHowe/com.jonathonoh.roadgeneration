using System.Collections.Generic;
using JonathonOH.RoadGeneration.ConvexShape2D;
using UnityEngine;

namespace JonathonOH.ConvexShapeExtruded
{
	public readonly struct ConvexHullExtruded
	{
		public readonly ConvexHull HorizontalHull;
		public readonly FloatRange VerticalRange;

		public ConvexHullExtruded(ConvexHull horizontalHull, FloatRange verticalRange)
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
				horizontalVertices.Add(new Vector2(vertex.x, vertex.y));
				if (!infiniteHeight)
				{
					maxVerticalPosition = Mathf.Max(maxVerticalPosition, vertex.y);
					minVerticalPosition = Mathf.Min(minVerticalPosition, vertex.y);
				}
			}

			FloatRange verticalRange = new FloatRange(minVerticalPosition, maxVerticalPosition);
			ConvexHull horizontalHull = new ConvexHull(horizontalVertices);
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
