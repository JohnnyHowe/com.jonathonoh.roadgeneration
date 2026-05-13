using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public readonly struct ConvexHull
	{
		public readonly IReadOnlyCollection<Vector2> Vertices;
		public readonly IReadOnlyCollection<Vector2> Tangents;
		public readonly IReadOnlyCollection<Vector2> Normals;

		public ConvexHull(IEnumerable<Vector2> allVertices)
		{
			Vertices = ConvexHullCalculator.GetConvexHull(allVertices).ToList();
			Tangents = ConvexHullUtility.GetTangentsFromHull(Vertices).ToList();
			Normals = ConvexHullUtility.GetNormalsFromTangents(Tangents).ToList();
		}

		public FloatRange GetProjection(Vector2 axis)
		{
			float min = Mathf.Infinity;
			float max = -Mathf.Infinity;
			foreach (Vector2 vertex in Vertices)
			{
				float vertexProjection = _Project(axis, vertex);
				min = Mathf.Min(min, vertexProjection);
				max = Mathf.Max(max, vertexProjection);
			}
			return new FloatRange(min, max);
		}

		private static float _Project(Vector2 axis, Vector2 point)
		{
			return Vector2.Dot(axis, point) / axis.magnitude;
		}

		public bool DoesOverlapWith(ConvexHull other)
		{
			// for each axis of both objects
			IEnumerable<Vector2> axes = Tangents.Concat(other.Tangents).Distinct().ToList();
			foreach (Vector2 axis in axes)
			{
				// get the projection of each object on the axis
				FloatRange thisProjectionRange = GetProjection(axis);
				FloatRange otherProjectionRange = other.GetProjection(axis);
				// if there is a gap, return false - there is no overlap
				if (!thisProjectionRange.OverlapsWith(otherProjectionRange)) return false;
			}
			return true;
		}
	}
}
