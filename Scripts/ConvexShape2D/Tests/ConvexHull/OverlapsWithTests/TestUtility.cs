using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullTests
{
	internal static class TestUtility
	{
		public static ConvexHull CreateHull(params Vector2[] points)
		{
			return new ConvexHull(points);
		}

		public static void AssertOverlapIsSymmetric(ConvexHull first, ConvexHull second, bool expected)
		{
			bool forward = first.OverlapsWith(second);
			bool reverse = second.OverlapsWith(first);

			Assert.That(forward, Is.EqualTo(expected));
			Assert.That(reverse, Is.EqualTo(expected));
			Assert.That(forward, Is.EqualTo(reverse));
		}
	}
}
