using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullTests
{
	internal class CanonicalShapeTests
	{
		[Test]
		public void IdenticalRectangles_ReturnsTrue()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));

			Assert.That(first.OverlapsWith(second), Is.True);
		}

		[Test]
		public void OneRectangleFullyInsideAnother_ReturnsTrue()
		{
			ConvexHull outer = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(4f, 0f),
				new Vector2(4f, 4f),
				new Vector2(0f, 4f));
			ConvexHull inner = TestUtility.CreateHull(
				new Vector2(1f, 1f),
				new Vector2(2f, 1f),
				new Vector2(2f, 2f),
				new Vector2(1f, 2f));

			Assert.That(outer.OverlapsWith(inner), Is.True);
		}

		[Test]
		public void RectanglesPartiallyOverlap_ReturnsTrue()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(3f, 0f),
				new Vector2(3f, 2f),
				new Vector2(0f, 2f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(2f, 1f),
				new Vector2(5f, 1f),
				new Vector2(5f, 3f),
				new Vector2(2f, 3f));

			Assert.That(first.OverlapsWith(second), Is.True);
		}

		[Test]
		public void RectanglesSeparatedAlongXAxis_ReturnsFalse()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(3f, 0f),
				new Vector2(5f, 0f),
				new Vector2(5f, 1f),
				new Vector2(3f, 1f));

			Assert.That(first.OverlapsWith(second), Is.False);
		}

		[Test]
		public void RectanglesSeparatedAlongYAxis_ReturnsFalse()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(0f, 2f),
				new Vector2(2f, 2f),
				new Vector2(2f, 3f),
				new Vector2(0f, 3f));

			Assert.That(first.OverlapsWith(second), Is.False);
		}

		[Test]
		public void OverlappingTriangles_ReturnsTrue()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(3f, 0f),
				new Vector2(1f, 2f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(1f, 0.5f),
				new Vector2(4f, 0.5f),
				new Vector2(2f, 3f));

			Assert.That(first.OverlapsWith(second), Is.True);
		}

		[Test]
		public void SeparatedTriangles_ReturnsFalse()
		{
			ConvexHull first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(1f, 1f));
			ConvexHull second = TestUtility.CreateHull(
				new Vector2(3f, 0f),
				new Vector2(5f, 0f),
				new Vector2(4f, 1f));

			Assert.That(first.OverlapsWith(second), Is.False);
		}
	}
}
