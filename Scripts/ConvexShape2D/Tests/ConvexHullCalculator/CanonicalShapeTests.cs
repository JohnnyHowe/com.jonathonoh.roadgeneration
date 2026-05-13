using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullCalculatorTests
{
	internal class CanonicalShapeTests
	{
		[Test]
		public void TriangleWithInteriorPoint_ReturnsTriangleVerticesOnly()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0.25f)
			};

			Vector2[] result = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(1f, 1f),
				new Vector2(2f, 0f),
				new Vector2(0f, 0f)
			}));
		}

		[Test]
		public void RectangleWithInteriorPoints_ReturnsFourCornersClockwise()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0.5f),
				new Vector2(0.5f, 0.25f)
			};

			Vector2[] result = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(0f, 1f),
				new Vector2(2f, 1f),
				new Vector2(2f, 0f),
				new Vector2(0f, 0f)
			}));
		}

		[Test]
		public void ConcavePointSet_ExcludesInwardNotchPoint()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(3f, 0f),
				new Vector2(3f, 3f),
				new Vector2(0f, 3f),
				new Vector2(1.5f, 1f),
				new Vector2(2f, 1.5f)
			};

			Vector2[] result = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(0f, 3f),
				new Vector2(3f, 3f),
				new Vector2(3f, 0f),
				new Vector2(0f, 0f)
			}));
		}

		[Test]
		public void MixedNegativeAndPositiveCoordinates_ReturnsCorrectHull()
		{
			Vector2[] points =
			{
				new Vector2(-2f, 0f),
				new Vector2(0f, -2f),
				new Vector2(2f, 0f),
				new Vector2(0f, 2f),
				new Vector2(0f, 0f),
				new Vector2(0.25f, -0.25f)
			};

			Vector2[] result = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(0f, 2f),
				new Vector2(2f, 0f),
				new Vector2(0f, -2f),
				new Vector2(-2f, 0f)
			}));
		}
	}
}
