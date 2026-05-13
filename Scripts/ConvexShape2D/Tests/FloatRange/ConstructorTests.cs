
using NUnit.Framework;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests
{
	internal class ConstructorTests
	{
		[Test]
		public void WhenFirstValueIsLessThanSecond_SetsMinAndMaxDirectly()
		{
			FloatRange range = new FloatRange(1f, 3f);

			Assert.That(range.Min, Is.EqualTo(1f));
			Assert.That(range.Max, Is.EqualTo(3f));
		}

		[Test]
		public void WhenFirstValueIsGreaterThanSecond_NormalizesMinAndMax()
		{
			FloatRange range = new FloatRange(3f, 1f);

			Assert.That(range.Min, Is.EqualTo(1f));
			Assert.That(range.Max, Is.EqualTo(3f));
		}

		[Test]
		public void WhenValuesAreEqual_CreatesSinglePointRange()
		{
			FloatRange range = new FloatRange(2f, 2f);

			Assert.That(range.Min, Is.EqualTo(2f));
			Assert.That(range.Max, Is.EqualTo(2f));
		}

		[Test]
		public void WhenBothValuesAreNegative_NormalizesCorrectly()
		{
			FloatRange range = new FloatRange(-2f, -5f);

			Assert.That(range.Min, Is.EqualTo(-5f));
			Assert.That(range.Max, Is.EqualTo(-2f));
		}

		[Test]
		public void WhenRangeCrossesZero_NormalizesCorrectly()
		{
			FloatRange range = new FloatRange(4f, -1f);

			Assert.That(range.Min, Is.EqualTo(-1f));
			Assert.That(range.Max, Is.EqualTo(4f));
		}

		[Test]
		public void WhenOneValueIsZero_SetsBoundsCorrectly()
		{
			FloatRange range = new FloatRange(0f, -5f);

			Assert.That(range.Min, Is.EqualTo(-5f));
			Assert.That(range.Max, Is.EqualTo(0f));
		}
	}
}
