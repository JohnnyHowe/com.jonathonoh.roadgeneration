using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullCalculatorTests
{
	/// <summary>
	/// Verifies deterministic hull ordering, clockwise winding, and basic output invariants.
	/// </summary>
	internal class OrderingTests
	{
		[Test]
		public void SamePointSetInDifferentOrders_ReturnsSameHullSequence()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0.5f)
			};

			Vector2[] reverseOrder = points.Reverse().ToArray();
			Vector2[] randomOrder =
			{
				points[2],
				points[0],
				points[4],
				points[3],
				points[1]
			};

			Vector2[] baselineHull = ConvexHullCalculator.GetConvexHull(points).ToArray();
			Vector2[] reverseHull = ConvexHullCalculator.GetConvexHull(reverseOrder).ToArray();
			Vector2[] randomHull = ConvexHullCalculator.GetConvexHull(randomOrder).ToArray();

			Assert.That(reverseHull, Is.EqualTo(baselineHull));
			Assert.That(randomHull, Is.EqualTo(baselineHull));
		}

		[Test]
		public void NonTrivialHull_IsReturnedInClockwiseOrder()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0.5f)
			};

			Vector2[] hull = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(GetSignedArea(hull), Is.LessThan(0f));
		}

		[Test]
		public void WhenMultipleVerticesShareLeftmostX_StartsAtUpperLeftmostVertex()
		{
			Vector2[] points =
			{
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0.5f)
			};

			Vector2[] hull = ConvexHullCalculator.GetConvexHull(points).ToArray();

			Assert.That(hull[0], Is.EqualTo(new Vector2(0f, 1f)));
		}

		[Test]
		public void HullVerticesAreAlwaysDrawnFromTheInputSet()
		{
			Vector2[] points =
			{
				new Vector2(-1f, 0f),
				new Vector2(0f, -2f),
				new Vector2(2f, -1f),
				new Vector2(3f, 2f),
				new Vector2(0f, 3f),
				new Vector2(0.5f, 0.75f)
			};

			Vector2[] hull = ConvexHullCalculator.GetConvexHull(points).ToArray();
			HashSet<Vector2> inputSet = points.ToHashSet();

			Assert.That(hull.All(inputSet.Contains), Is.True);
		}

		private static float GetSignedArea(IReadOnlyList<Vector2> polygon)
		{
			float twiceArea = 0f;
			for (int i = 0; i < polygon.Count; i++)
			{
				Vector2 current = polygon[i];
				Vector2 next = polygon[(i + 1) % polygon.Count];
				twiceArea += current.x * next.y - next.x * current.y;
			}

			return twiceArea * 0.5f;
		}
	}
}
