using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullUtilityTests
{
	/// <summary>
	/// Verifies tangent generation for representative clockwise hull shapes.
	/// </summary>
	internal class GetTangentsFromHullCanonicalShapeTests
	{
		[Test]
		public void Triangle_ReturnsOneTangentPerEdgeIncludingClosingEdge()
		{
			Vector2[] hullClockwise =
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 0f),
				new Vector2(-1f, 0f)
			};

			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(hullClockwise).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(1f, -1f).normalized,
				new Vector2(-2f, 0f).normalized,
				new Vector2(1f, 1f).normalized
			}).Using(Vector2EqualityComparer.Instance));
		}

		[Test]
		public void Rectangle_ReturnsTangentsInHullOrder()
		{
			Vector2[] hullClockwise =
			{
				new Vector2(0f, 1f),
				new Vector2(2f, 1f),
				new Vector2(2f, 0f),
				new Vector2(0f, 0f)
			};

			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(hullClockwise).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				Vector2.right,
				Vector2.down,
				Vector2.left,
				Vector2.up
			}).Using(Vector2EqualityComparer.Instance));
		}
	}
}
