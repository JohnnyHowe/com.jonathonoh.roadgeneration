using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ConvexHullUtilityTests
{
	/// <summary>
	/// Verifies general invariants of generated hull tangents.
	/// </summary>
	internal class GetTangentsFromHullInvariantTests
	{
		[Test]
		public void TangentsAreNormalized()
		{
			Vector2[] hullClockwise =
			{
				new Vector2(-1f, 2f),
				new Vector2(3f, 2f),
				new Vector2(2f, -1f)
			};

			Vector2[] result = ConvexHullUtility.GetTangentsFromHull(hullClockwise).ToArray();

			Assert.That(result, Is.Not.Empty);
			Assert.That(result.All(tangent => Mathf.Approximately(tangent.magnitude, 1f)), Is.True);
			Assert.That(result.Distinct(TangentComparer.Instance).Count(), Is.EqualTo(result.Length));
		}
	}
}
