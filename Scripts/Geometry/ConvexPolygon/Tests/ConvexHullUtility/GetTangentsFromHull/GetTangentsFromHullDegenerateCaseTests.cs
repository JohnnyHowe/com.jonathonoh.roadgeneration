using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.ConvexHullUtilityTests
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

			Vector2[] expected = new[]
			{
				Vector2.right,
				Vector2.up
			};

			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(hullClockwise).ToArray();
			Assert.That(result, Is.EqualTo(expected).Using(TangentComparer.Instance));
		}

		[Test]
		public void AdjacentVerticesThatNormalizeToZero_SkipsEdge()
		{
			Vector2[] hullClockwise =
			{
				new Vector2(0f, 6.520003f),
				new Vector2(1f, 6.520003f),
				new Vector2(1f, 6.520002f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),
			};

			Vector2[] expected = new[]
			{
				Vector2.right,
				Vector2.up
			};

			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(hullClockwise).ToArray();
			Assert.That(result, Is.EqualTo(expected).Using(TangentComparer.Instance));
		}


		[Test]
		public void ClosingVerticesThatNormalizeToZero_SkipsEdge()
		{
			Vector2[] hullClockwise =
			{
				new Vector2(3.48f, 3.480001f),
				new Vector2(3.480002f, -3.479997f),
				new Vector2(-3.479998f, -3.479998f),
				new Vector2(-3.48f, 3.48f)
			};

			Vector2[] expected = new[]
			{
				Vector2.right,
				Vector2.up
			};

			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(hullClockwise).ToArray();
			Assert.That(result, Is.EquivalentTo(expected).Using(TangentComparer.Instance));
		}
	}
}
