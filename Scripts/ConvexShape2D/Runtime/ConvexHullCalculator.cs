
using System;
using System.Collections.Generic;
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
	}
}
 