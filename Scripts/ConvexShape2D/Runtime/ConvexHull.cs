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

		public bool OverlapsWith(ConvexHull other)
		{
			// for each normal of both objects
			IEnumerable<Vector2> allnormals = Normals.Concat(other.Normals).Distinct(TangentComparer.Instance).ToList();

			foreach (Vector2 axis in allnormals)
			{
				if (DoesOverlapWith(other, axis))
				{
					return true;
				}
			}
			return false;
		}

		private bool DoesOverlapWith(ConvexHull other, Vector2 axis)
		{
			// get the projection of each object on the axis
			FloatRange thisProjectionRange = ProjectionUtility.ProjectAll(Vertices, axis);
			FloatRange otherProjectionRange = ProjectionUtility.ProjectAll(other.Vertices, axis);

			// if there is a gap, return false - there is no overlap
			return thisProjectionRange.OverlapsWith(otherProjectionRange);
		}
	}
}
