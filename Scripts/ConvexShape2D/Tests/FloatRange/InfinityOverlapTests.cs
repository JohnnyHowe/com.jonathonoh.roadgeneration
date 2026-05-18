using NUnit.Framework;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests.FloatRangeTests
{
	internal class InfinityOverlapTests
	{
		[Test]
		public void NegativeInfinityMinimumOverlapsFiniteRangeBelowMax_ReturnsTrue()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, 3f);
			FloatRange other = new FloatRange(1f, 5f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void NegativeInfinityMinimumSeparatedFromFiniteRangeAboveMax_ReturnsFalse()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, 3f);
			FloatRange other = new FloatRange(4f, 5f);

			Assert.That(range.OverlapsWith(other), Is.False);
		}

		[Test]
		public void NegativeInfinityMinimumTouchingFiniteRangeAtMax_ReturnsTrue()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, 3f);
			FloatRange other = new FloatRange(3f, 5f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void PositiveInfinityMaximumOverlapsFiniteRangeAboveMin_ReturnsTrue()
		{
			FloatRange range = new FloatRange(3f, float.PositiveInfinity);
			FloatRange other = new FloatRange(1f, 5f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void PositiveInfinityMaximumSeparatedFromFiniteRangeBelowMin_ReturnsFalse()
		{
			FloatRange range = new FloatRange(3f, float.PositiveInfinity);
			FloatRange other = new FloatRange(1f, 2f);

			Assert.That(range.OverlapsWith(other), Is.False);
		}

		[Test]
		public void PositiveInfinityMaximumTouchingFiniteRangeAtMin_ReturnsTrue()
		{
			FloatRange range = new FloatRange(3f, float.PositiveInfinity);
			FloatRange other = new FloatRange(1f, 3f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void FullyInfiniteRangeOverlapsFiniteRange_ReturnsTrue()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, float.PositiveInfinity);
			FloatRange other = new FloatRange(1f, 2f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void FullyInfiniteRangeOverlapsNegativeInfiniteRange_ReturnsTrue()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, float.PositiveInfinity);
			FloatRange other = new FloatRange(float.NegativeInfinity, 2f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void NegativeInfiniteRangeSeparatedFromPositiveInfiniteRange_ReturnsFalse()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, -1f);
			FloatRange other = new FloatRange(1f, float.PositiveInfinity);

			Assert.That(range.OverlapsWith(other), Is.False);
		}

		[Test]
		public void NegativeInfiniteRangeTouchingPositiveInfiniteRange_ReturnsTrue()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, 0f);
			FloatRange other = new FloatRange(0f, float.PositiveInfinity);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void CalledInEitherDirectionForInfiniteOverlappingRanges_ReturnsSameResult()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, 3f);
			FloatRange other = new FloatRange(1f, float.PositiveInfinity);

			Assert.That(range.OverlapsWith(other), Is.EqualTo(other.OverlapsWith(range)));
		}

		[Test]
		public void CalledInEitherDirectionForInfiniteSeparatedRanges_ReturnsSameResult()
		{
			FloatRange range = new FloatRange(float.NegativeInfinity, -1f);
			FloatRange other = new FloatRange(1f, float.PositiveInfinity);

			Assert.That(range.OverlapsWith(other), Is.EqualTo(other.OverlapsWith(range)));
		}
	}
}
