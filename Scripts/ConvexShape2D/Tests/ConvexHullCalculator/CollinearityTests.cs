using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullCalculatorTests
{
	internal class CollinearityTests
	{
		[Test]
		public void AllPointsAreCollinear_HorizontalLine_ReturnsOnlyExtremeEndpoints()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(2f, 0f),
				new Vector2(3f, 0f)
			};

			Vector2[] result = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(3f, 0f),
				new Vector2(0f, 0f)
			}));
		}

		[Test]
		public void AllPointsAreCollinear_DiagonalLine_ReturnsOnlyExtremeEndpoints()
		{
			Vector2[] points =
			{
				new Vector2(-1f, -1f),
				new Vector2(0f, 0f),
				new Vector2(1f, 1f),
				new Vector2(2f, 2f)
			};

			Vector2[] result = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(2f, 2f),
				new Vector2(-1f, -1f)
			}));
		}

		[Test]
		public void CollinearPointsOnRectangleEdge_ExcludeIntermediateEdgePoints()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f)
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
	}
}
