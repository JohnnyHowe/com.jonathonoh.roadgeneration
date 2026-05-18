using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.ConvexPolygonTests
{
	internal static class TestUtility
	{
		public static ConvexPolygon CreateHull(params Vector2[] points)
		{
			return new ConvexPolygon(points);
		}

		public static void AssertOverlapIsSymmetric(ConvexPolygon first, ConvexPolygon second, bool expected)
		{
			bool forward = first.OverlapsWith(second);
			bool reverse = second.OverlapsWith(first);

			Assert.That(forward, Is.EqualTo(expected));
			Assert.That(reverse, Is.EqualTo(expected));
			Assert.That(forward, Is.EqualTo(reverse));
		}
	}
}
