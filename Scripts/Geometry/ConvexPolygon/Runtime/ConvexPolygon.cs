using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.Geometry
{
	public readonly struct ConvexPolygon
	{
		public readonly IReadOnlyList<Vector2> Vertices;
		public readonly IReadOnlyList<Vector2> Tangents;
		public readonly IReadOnlyList<Vector2> Normals;
		public readonly Rect BoundingRect;

		public ConvexPolygon(IEnumerable<Vector2> allVertices)
		{
			Vertices = ConvexHullCalculator.GetConvexHull(allVertices).ToList();

			if (Vertices.Count == 0)
			{
				throw new ArgumentException("Cannot create a convex hull with zero vertices!");
			}

			Tangents = ConvexHullUtility.GetTangentsFromHull(Vertices).ToList();
			Normals = ConvexHullUtility.GetNormalsFromTangents(Tangents).ToList();
			BoundingRect = BoundingRectCalculator.FromVertices(Vertices);
		}

		/// <summary>
		/// Returns true when this hull overlaps another hull, including when they only touch at a boundary.
		/// </summary>
		public bool OverlapsWith(ConvexPolygon other)
		{
			if (!OverlapsWithBoundaries(other.BoundingRect))
			{
				return false;
			}
			return OverlapsWithUsingSAT(other);
		}

		private bool OverlapsWithBoundaries(Rect other)
		{
			return (
				BoundingRect.xMin <= other.xMax &&
				BoundingRect.xMax >= other.xMin &&
				BoundingRect.yMin <= other.yMax &&
				BoundingRect.yMax >= other.yMin
			);
		}

		private bool OverlapsWithUsingSAT(ConvexPolygon other)
		{
			IEnumerable<Vector2> allNormals = Normals.Concat(other.Normals).Distinct(TangentComparer.Instance);
			foreach (Vector2 axis in allNormals)
			{
				if (!ProjectionOverlapsOnAxis(other, axis))
				{
					return false;
				}
			}
			return true;
		}

		private bool ProjectionOverlapsOnAxis(ConvexPolygon other, Vector2 axis)
		{
			FloatRange thisProjectionRange = ProjectionUtility.ProjectAll(Vertices, axis);
			FloatRange otherProjectionRange = ProjectionUtility.ProjectAll(other.Vertices, axis);
			return thisProjectionRange.OverlapsWith(otherProjectionRange);
		}

		public override string ToString()
		{
			return $"ConvexHull<Center={BoundingRect.center}, size={BoundingRect.size}>";
		}
	}
}
