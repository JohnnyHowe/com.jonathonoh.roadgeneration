using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullCalculatorTests
{
	/// <summary>
	/// Verifies the early-return behavior for empty, single-point, and two-point inputs.
	/// </summary>
	internal class TrivialCaseTests
	{
		[Test]
		public void WhenInputIsEmpty_ReturnsEmptyHull()
		{
			Vector2[] result = ConvexHullCalculator.GetConvexHull(Enumerable.Empty<Vector2>()).ToArray();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void WhenInputHasOnePoint_ReturnsThatPoint()
		{
			Vector2 point = new Vector2(2f, 3f);

			Vector2[] result = ConvexHullCalculator.GetConvexHull(new[] { point }).ToArray();

			Assert.That(result, Is.EqualTo(new[] { point }));
		}

		[Test]
		public void WhenInputHasTwoDistinctPoints_ReturnsBothPoints()
		{
			Vector2 first = new Vector2(-1f, 0f);
			Vector2 second = new Vector2(4f, 2f);

			Vector2[] result = ConvexHullCalculator.GetConvexHull(new[] { first, second }).ToArray();

			Assert.That(result, Is.EqualTo(new[] { first, second }));
		}
	}
}
