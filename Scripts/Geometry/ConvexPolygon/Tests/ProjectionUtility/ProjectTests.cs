using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.ProjectionUtilityTests
{
	internal class ProjectTests
	{
		[Test]
		public void PointLiesAlongPositiveAxis_ReturnsPositiveDistance()
		{
			float result = ProjectionUtility.Project(new Vector2(3f, 0f), Vector2.right);

			Assert.That(result, Is.EqualTo(3f));
		}

		[Test]
		public void PointLiesOppositeAxisDirection_ReturnsNegativeDistance()
		{
			float result = ProjectionUtility.Project(new Vector2(-2f, 0f), Vector2.right);

			Assert.That(result, Is.EqualTo(-2f));
		}

		[Test]
		public void PointIsPerpendicularToAxis_ReturnsZero()
		{
			float result = ProjectionUtility.Project(new Vector2(0f, 5f), Vector2.right);

			Assert.That(result, Is.EqualTo(0f));
		}

		[Test]
		public void AxisIsNotNormalized_ReturnsProjectionUsingAxisDirectionOnly()
		{
			float result = ProjectionUtility.Project(new Vector2(3f, 4f), new Vector2(0f, 2f));

			Assert.That(result, Is.EqualTo(4f));
		}
	}
}
