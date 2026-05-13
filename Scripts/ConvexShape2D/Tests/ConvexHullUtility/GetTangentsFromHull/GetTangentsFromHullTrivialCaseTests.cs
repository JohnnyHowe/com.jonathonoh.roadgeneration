using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullUtilityTests
{
	/// <summary>
	/// Verifies tangent generation for empty and minimal hull inputs.
	/// </summary>
	internal class GetTangentsFromHullTrivialCaseTests
	{
		[Test]
		public void EmptyHull_ReturnsEmpty()
		{
			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(Enumerable.Empty<Vector2>()).ToArray();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void SingleVertexHull_ReturnsEmpty()
		{
			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(new[]
			{
				new Vector2(2f, 3f)
			}).ToArray();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void TwoVertexHull_ReturnsForwardNormalizedTangent()
		{
			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(new[]
			{
				new Vector2(1f, 1f),
				new Vector2(4f, 5f)
			}).ToArray();

			Assert.That(result, Is.EqualTo(new[]
			{
				new Vector2(3f, 4f).normalized
			}).Using(Vector2EqualityComparer.Instance));
		}
	}
}
