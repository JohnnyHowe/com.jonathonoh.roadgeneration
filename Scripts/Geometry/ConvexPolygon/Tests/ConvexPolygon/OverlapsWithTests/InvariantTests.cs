using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.ConvexPolygonTests
{
	/// <summary>
	/// Verifies invariants of overlap testing, meaning properties that should not change when the same shapes are
	/// represented differently or when the overlap query is evaluated in the opposite direction.
	/// </summary>
	internal class InvariantTests
	{
		[Test]
		public void OverlapCheckIsSymmetricForOverlappingShapes()
		{
			ConvexPolygon first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(3f, 0f),
				new Vector2(3f, 2f),
				new Vector2(0f, 2f));
			ConvexPolygon second = TestUtility.CreateHull(
				new Vector2(2f, 1f),
				new Vector2(5f, 1f),
				new Vector2(5f, 3f),
				new Vector2(2f, 3f));

			TestUtility.AssertOverlapIsSymmetric(first, second, true);
		}

		[Test]
		public void OverlapCheckIsSymmetricForSeparatedShapes()
		{
			ConvexPolygon first = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexPolygon second = TestUtility.CreateHull(
				new Vector2(3f, 0f),
				new Vector2(5f, 0f),
				new Vector2(5f, 1f),
				new Vector2(3f, 1f));

			TestUtility.AssertOverlapIsSymmetric(first, second, false);
		}

		[Test]
		public void HullConstructedFromUnorderedPoints_ProducesSameOverlapResult()
		{
			ConvexPolygon unorderedHull = TestUtility.CreateHull(
				new Vector2(2f, 1f),
				new Vector2(0f, 0f),
				new Vector2(1f, 0.5f),
				new Vector2(0f, 1f),
				new Vector2(2f, 0f));
			ConvexPolygon orderedHull = TestUtility.CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			ConvexPolygon other = TestUtility.CreateHull(
				new Vector2(1.5f, 0.25f),
				new Vector2(3f, 0.25f),
				new Vector2(3f, 1.5f),
				new Vector2(1.5f, 1.5f));

			bool unorderedResult = unorderedHull.OverlapsWith(other);
			bool orderedResult = orderedHull.OverlapsWith(other);

			Assert.That(unorderedResult, Is.EqualTo(orderedResult));
		}
	}
}
