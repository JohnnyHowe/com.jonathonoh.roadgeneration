using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullUtilityTests
{
	/// <summary>
	/// Verifies tangent generation for duplicate and collinear edge cases in the supplied hull.
	/// </summary>
	internal class GetTangentsFromHullDegenerateCaseTests
	{
		[Test]
		public void DuplicateAdjacentVertices_SkipsZeroLengthEdge()
		{
			Vector2[] hullClockwise =
			{
				new Vector2(0f, 1f),
				new Vector2(2f, 1f),
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

		[Test]
		public void CollinearAdjacentEdges_PreservesOneTangentPerEdgeSegment()
		{
			Vector2[] hullClockwise =
			{
				new Vector2(0f, 1f),
				new Vector2(2f, 1f),
				new Vector2(2f, 0.5f),
				new Vector2(2f, 0f),
				new Vector2(0f, 0f)
			};

			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(hullClockwise).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				Vector2.right,
				Vector2.down,
				Vector2.down,
				Vector2.left,
				Vector2.up
			}).Using(Vector2EqualityComparer.Instance));
		}
	}
}
