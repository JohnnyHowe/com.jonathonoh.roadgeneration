using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.TangentEqualityComparerTests
{
	/// <summary>
	/// Verifies tangent equality behavior for equivalent and opposite-facing tangent vectors.
	/// </summary>
	internal class OppositeTests
	{
		[Test]
		public void LeftRight_AreEqual()
		{
			Vector2 tangent = Vector2.right;
			Vector2 oppositeTangent = Vector2.left;

			bool result = TangentComparer.Instance.Equals(tangent, oppositeTangent);

			Assert.That(result, Is.True);
		}
	}
}
