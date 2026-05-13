using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullCalculatorTests
{
	internal class DuplicatePointTests
	{
		[Test]
		public void DuplicateCornersAndInteriorPoints_DoNotCreateDuplicateHullVertices()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f),
				new Vector2(0f, 0f),
				new Vector2(2f, 1f),
				new Vector2(1f, 0.5f),
				new Vector2(1f, 0.5f)
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
		public void ThreeIdenticalPoints_ReturnsTwoCopiesOfThatPoint()
		{
			Vector2 point = new Vector2(2f, 3f);

			Vector2[] result = ConvexHullCalculator.GetConvexHull(new[] { point, point, point }).ToArray();

			Assert.That(result, Is.EqualTo(new[] { point, point }));
		}

		[Test]
		public void TwoUniquePointsWithDuplicates_ReturnsTheExtremeEndpoints()
		{
			Vector2 left = new Vector2(-1f, 0f);
			Vector2 right = new Vector2(4f, 2f);

			Vector2[] result = ConvexHullCalculator.GetConvexHull(new[] { left, right, left, right }).ToArray();

			Assert.That(result, Is.EqualTo(new[] { right, left }));
		}
	}
}
