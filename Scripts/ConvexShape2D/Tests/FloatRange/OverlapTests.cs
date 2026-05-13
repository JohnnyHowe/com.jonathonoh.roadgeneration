using NUnit.Framework;

namespace JonathonOH.RoadGeneration.ConvexShape2D.Tests
{
	internal class OverlapTests
	{
		[Test]
		public void RangesAreIdentical_ReturnsTrue()
		{
			FloatRange range = new FloatRange(1f, 3f);
			FloatRange other = new FloatRange(1f, 3f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void OneRangeContainsTheOther_ReturnsTrue()
		{
			FloatRange range = new FloatRange(1f, 5f);
			FloatRange other = new FloatRange(2f, 4f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void RangesPartiallyOverlapOnLeft_ReturnsTrue()
		{
			FloatRange range = new FloatRange(1f, 4f);
			FloatRange other = new FloatRange(0f, 2f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void RangesPartiallyOverlapOnRight_ReturnsTrue()
		{
			FloatRange range = new FloatRange(1f, 4f);
			FloatRange other = new FloatRange(3f, 6f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void RangesTouchAtBoundary_ReturnsTrue()
		{
			FloatRange range = new FloatRange(1f, 3f);
			FloatRange other = new FloatRange(3f, 5f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void RangesAreSeparated_ReturnsFalse()
		{
			FloatRange range = new FloatRange(1f, 2f);
			FloatRange other = new FloatRange(3f, 4f);

			Assert.That(range.OverlapsWith(other), Is.False);
		}

		[Test]
		public void NegativeAndPositiveRangesAreSeparated_ReturnsFalse()
		{
			FloatRange range = new FloatRange(-5f, -1f);
			FloatRange other = new FloatRange(0f, 2f);

			Assert.That(range.OverlapsWith(other), Is.False);
		}

		[Test]
		public void PointRangeLiesInsideOtherRange_ReturnsTrue()
		{
			FloatRange range = new FloatRange(2f, 2f);
			FloatRange other = new FloatRange(1f, 3f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void TwoPointRangesShareSameValue_ReturnsTrue()
		{
			FloatRange range = new FloatRange(2f, 2f);
			FloatRange other = new FloatRange(2f, 2f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void TwoPointRangesDiffer_ReturnsFalse()
		{
			FloatRange range = new FloatRange(2f, 2f);
			FloatRange other = new FloatRange(3f, 3f);

			Assert.That(range.OverlapsWith(other), Is.False);
		}

		[Test]
		public void ThisRangeWasConstructedInReverse_UsesNormalizedBounds()
		{
			FloatRange range = new FloatRange(4f, 1f);
			FloatRange other = new FloatRange(3f, 5f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void OtherRangeWasConstructedInReverse_UsesNormalizedBounds()
		{
			FloatRange range = new FloatRange(1f, 4f);
			FloatRange other = new FloatRange(5f, 3f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void BothRangesWereConstructedInReverse_UsesNormalizedBounds()
		{
			FloatRange range = new FloatRange(4f, 1f);
			FloatRange other = new FloatRange(5f, 3f);

			Assert.That(range.OverlapsWith(other), Is.True);
		}

		[Test]
		public void CalledInEitherDirectionForOverlappingRanges_ReturnsSameResult()
		{
			FloatRange range = new FloatRange(1f, 4f);
			FloatRange other = new FloatRange(3f, 6f);

			Assert.That(range.OverlapsWith(other), Is.EqualTo(other.OverlapsWith(range)));
		}

		[Test]
		public void CalledInEitherDirectionForSeparatedRanges_ReturnsSameResult()
		{
			FloatRange range = new FloatRange(1f, 2f);
			FloatRange other = new FloatRange(3f, 4f);

			Assert.That(range.OverlapsWith(other), Is.EqualTo(other.OverlapsWith(range)));
		}
	}
}
