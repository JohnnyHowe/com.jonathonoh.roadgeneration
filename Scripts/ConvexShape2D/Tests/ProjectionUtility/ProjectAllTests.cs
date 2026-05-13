using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.ProjectionUtilityTests
{
	internal class ProjectAllTests
	{
		[Test]
		public void SinglePoint_ReturnsRangeAtProjectedValue()
		{
			FloatRange result = ProjectionUtility.ProjectAll(new[]
			{
				new Vector2(3f, 0f)
			}, Vector2.right);

			Assert.That(result.Min, Is.EqualTo(3f));
			Assert.That(result.Max, Is.EqualTo(3f));
		}

		[Test]
		public void MultiplePointsOnSameAxis_ReturnsMinAndMaxProjectedValues()
		{
			FloatRange result = ProjectionUtility.ProjectAll(new[]
			{
				new Vector2(-2f, 0f),
				new Vector2(1f, 0f),
				new Vector2(4f, 0f)
			}, Vector2.right);

			Assert.That(result.Min, Is.EqualTo(-2f));
			Assert.That(result.Max, Is.EqualTo(4f));
		}

		[Test]
		public void PointsPerpendicularToAxis_ReturnsZeroWidthRangeAtZero()
		{
			FloatRange result = ProjectionUtility.ProjectAll(new[]
			{
				new Vector2(0f, 1f),
				new Vector2(0f, -3f)
			}, Vector2.right);

			Assert.That(result.Min, Is.EqualTo(0f));
			Assert.That(result.Max, Is.EqualTo(0f));
		}

		[Test]
		public void AxisIsNotNormalized_UsesDirectionToComputeProjectionRange()
		{
			FloatRange result = ProjectionUtility.ProjectAll(new[]
			{
				new Vector2(3f, 4f),
				new Vector2(1f, -2f),
				new Vector2(-5f, 6f)
			}, new Vector2(0f, 2f));

			Assert.That(result.Min, Is.EqualTo(-2f));
			Assert.That(result.Max, Is.EqualTo(6f));
		}
	}
}
