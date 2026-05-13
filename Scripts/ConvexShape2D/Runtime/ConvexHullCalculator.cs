
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public static class ConvexHullCalculator
	{
		/// <summary>
		/// Uses Andrew's monotone chain convex hull algorithm.
		/// See https://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain.
		/// 
		/// Returns points Clockwise from leftmost point (lowest x).
		/// </summary>
		public static IEnumerable<Vector2> GetConvexHull(IEnumerable<Vector2> unorderedPoints)
		{
			throw new NotImplementedException();
		}

		
		/// <summary>
		/// Returns the input points sorted lexicographically by ascending x, then ascending y for equal x values.
		/// </summary>
		private static IEnumerable<Vector2> GetPointsSortedLexicographically(IEnumerable<Vector2> unorderedPoints)
		{
			return unorderedPoints
				.OrderBy(point => point.x)
				.ThenBy(point => point.y);
		}

		/// <summary>
		/// Returns the z-component of the 2D cross product of <paramref name="fromOriginToA"/> and <paramref name="fromOriginToB"/>.
		/// Positive values indicate a counterclockwise turn, negative values indicate a clockwise turn, and zero indicates collinearity.
		/// </summary>
		private static float Cross(Vector2 origin, Vector2 fromOriginToA, Vector2 fromOriginToB)
		{
			Vector2 a = fromOriginToA - origin;
			Vector2 b = fromOriginToB - origin;
			return a.x * b.y - a.y * b.x;
		}
	}
}
 
