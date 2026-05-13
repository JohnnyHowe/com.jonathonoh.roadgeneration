using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullTests
{
	internal class BoundaryContactTests
	{
		[Test]
		public void RectanglesTouchAtVerticalEdge_ReturnsTrue()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(2f, 0f),
				new Vector2(4f, 0f),
				new Vector2(4f, 1f),
				new Vector2(2f, 1f));

			TestUtility.AssertOverlapIsSymmetric(first, second, true);
		}

		[Test]
		public void RectanglesTouchAtHorizontalEdge_ReturnsTrue()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(0f, 1f),
				new Vector2(2f, 1f),
				new Vector2(2f, 3f),
				new Vector2(0f, 3f));

			TestUtility.AssertOverlapIsSymmetric(first, second, true);
		}

		[Test]
		public void RectanglesTouchAtSingleCorner_ReturnsTrue()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 2f),
				new Vector2(0f, 2f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(2f, 2f),
				new Vector2(4f, 2f),
				new Vector2(4f, 4f),
				new Vector2(2f, 4f));

			TestUtility.AssertOverlapIsSymmetric(first, second, true);
		}

		[Test]
		public void TriangleTouchingRectangleAtVertex_ReturnsTrue()
		{
			ConvexHull triangle = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(1f, 2f));
			ConvexHull rectangle = TestUtility.CreateHull(
				new Vector2(1f, 2f),
				new Vector2(3f, 2f),
				new Vector2(3f, 4f),
				new Vector2(1f, 4f));

			TestUtility.AssertOverlapIsSymmetric(triangle, rectangle, true);
		}
	}
}
