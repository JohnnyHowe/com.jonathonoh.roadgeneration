using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.ConvexPolygonTests
{
	internal class BoundaryContactTests
	{
		[Test]
		public void RectanglesTouchAtVerticalEdge_ReturnsTrue()
		{
			ConvexPolygon first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexPolygon second = TestUtility.CreateHull(
				new Vector2(2f, 0f),
				new Vector2(4f, 0f),
				new Vector2(4f, 1f),
				new Vector2(2f, 1f));

			TestUtility.AssertOverlapIsSymmetric(first, second, true);
		}

		[Test]
		public void RectanglesTouchAtHorizontalEdge_ReturnsTrue()
		{
			ConvexPolygon first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexPolygon second = TestUtility.CreateHull(
				new Vector2(0f, 1f),
				new Vector2(2f, 1f),
				new Vector2(2f, 3f),
				new Vector2(0f, 3f));

			TestUtility.AssertOverlapIsSymmetric(first, second, true);
		}

		[Test]
		public void RectanglesTouchAtSingleCorner_ReturnsTrue()
		{
			ConvexPolygon first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 2f),
				new Vector2(0f, 2f));
			ConvexPolygon second = TestUtility.CreateHull(
				new Vector2(2f, 2f),
				new Vector2(4f, 2f),
				new Vector2(4f, 4f),
				new Vector2(2f, 4f));

			TestUtility.AssertOverlapIsSymmetric(first, second, true);
		}

		[Test]
		public void TriangleTouchingRectangleAtVertex_ReturnsTrue()
		{
			ConvexPolygon triangle = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(1f, 2f));
			ConvexPolygon rectangle = TestUtility.CreateHull(
				new Vector2(1f, 2f),
				new Vector2(3f, 2f),
				new Vector2(3f, 4f),
				new Vector2(1f, 4f));

			TestUtility.AssertOverlapIsSymmetric(triangle, rectangle, true);
		}
	}
}
