using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullTests
{
	internal class RotatedShapeTests
	{
		[Test]
		public void RotatedDiamondOverlappingRectangle_ReturnsTrue()
		{
			ConvexHull rectangle = TestUtility.CreateHull(
				new Vector2(-1f, -1f),
				new Vector2(1f, -1f),
				new Vector2(1f, 1f),
				new Vector2(-1f, 1f));
			ConvexHull diamond = TestUtility.CreateHull(
				new Vector2(0f, 2f),
				new Vector2(2f, 0f),
				new Vector2(0f, -2f),
				new Vector2(-2f, 0f));

			Assert.That(rectangle.OverlapsWith(diamond), Is.True);
		}

		[Test]
		public void RotatedDiamondTouchingRectangle_ReturnsTrue()
		{
			ConvexHull rectangle = TestUtility.CreateHull(
				new Vector2(-1f, -1f),
				new Vector2(1f, -1f),
				new Vector2(1f, 1f),
				new Vector2(-1f, 1f));
			ConvexHull shiftedDiamond = TestUtility.CreateHull(
				new Vector2(3f, 2f),
				new Vector2(5f, 0f),
				new Vector2(3f, -2f),
				new Vector2(1f, 0f));

			Assert.That(rectangle.OverlapsWith(shiftedDiamond), Is.True);
		}

		[Test]
		public void RotatedDiamondSeparatedFromRectangle_ReturnsFalse()
		{
			ConvexHull rectangle = TestUtility.CreateHull(
				new Vector2(-1f, -1f),
				new Vector2(1f, -1f),
				new Vector2(1f, 1f),
				new Vector2(-1f, 1f));
			ConvexHull shiftedDiamond = TestUtility.CreateHull(
				new Vector2(4f, 2f),
				new Vector2(5f, 0f),
				new Vector2(4f, -2f),
				new Vector2(2f, 0f));

			Assert.That(rectangle.OverlapsWith(shiftedDiamond), Is.False);
		}

	}
}
