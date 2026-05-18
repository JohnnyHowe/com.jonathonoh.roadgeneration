using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.TangentEqualityComparerTests
{
	/// <summary>
	/// Verifies tangent equality behavior for equivalent and opposite-facing tangent vectors.
	/// </summary>
	internal class TooSmallDifferenceTests
	{
		[Test]
		public void UnitTangentsWithTooSmallDifference_AreEqual()
		{
			Vector2 tangent1 = new Vector2(0.999999f, 0);
			Vector2 tangent2 = new Vector2(0.999998f, 0);

			bool result = TangentComparer.Instance.Equals(tangent1, tangent2);
			Assert.That(result, Is.True);
		}

		[Test]
		public void NonUnitTangentsWithTooSmallDifference_AreEqual()
		{
			Vector2 tangent1 = new Vector2(3.480002f, 0);
			Vector2 tangent2 = new Vector2(3.479998f, 0);

			bool result = TangentComparer.Instance.Equals(tangent1, tangent2);
			Assert.That(result, Is.True);
		}

	}
}

