using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public readonly struct ConvexHull
	{
		public readonly IReadOnlyList<Vector2> Vertices;
		public readonly IReadOnlyList<Vector2> Tangents;
		public readonly IReadOnlyList<Vector2> Normals;

		public ConvexHull(IEnumerable<Vector2> allVertices)
		{
			Vertices = ConvexHullCalculator.GetConvexHull(allVertices).ToList();

			if (Vertices.Count == 0)
			{
				throw new ArgumentException("Cannot create a convex hull with zero vertices!");
			}

			Tangents = ConvexHullUtility.GetTangentsFromHull(Vertices).ToList();
			Normals = ConvexHullUtility.GetNormalsFromTangents(Tangents).ToList();
		}

		/// <summary>
        /// Includes when they only touch at a boundary -> true.
		/// </summary>
		public bool OverlapsWith(ConvexHull other)
		{
			IEnumerable<Vector2> allnormals = Normals.Concat(other.Normals).Distinct(TangentComparer.Instance);
			foreach (Vector2 axis in allnormals)
			{
				if (!OverlapsWith(other, axis))
				{
					return false;
				}
			}
			return true;
		}

		private bool OverlapsWith(ConvexHull other, Vector2 axis)
		{
			FloatRange thisProjectionRange = ProjectionUtility.ProjectAll(Vertices, axis);
			FloatRange otherProjectionRange = ProjectionUtility.ProjectAll(other.Vertices, axis);
			return thisProjectionRange.OverlapsWith(otherProjectionRange);
		}
	}
}
