using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JonathonOH.BacktrackableEnumerator.Tests
{
	public static class ConstructionTests
	{
		[Test]
		public static void Constructor_WhenSearchSpaceIsNonEmpty_DoesNotAdvanceTraversal()
		{
			bool getNextOptionsWasCalled = false;
			DFSEnumerator<int>.GetNextOptions getNextOptions = (IReadOnlyList<int> current) =>
			{
				getNextOptionsWasCalled = true;
				return new[] { 1 };
			};

			DFSEnumerator<int> enumerator = new DFSEnumerator<int>(getNextOptions);

			Assert.IsFalse(getNextOptionsWasCalled);
			CollectionAssert.IsEmpty(enumerator.Current);
		}

		[Test]
		public static void MoveNext_WhenSearchSpaceIsEmpty_ReturnsFalse()
		{
			DFSEnumerator<int>.GetNextOptions getNextOptions = (IReadOnlyList<int> current) => Array.Empty<int>();
			DFSEnumerator<int> enumerator = new DFSEnumerator<int>(getNextOptions);

			bool moved = enumerator.MoveNext();

			Assert.IsFalse(moved);
			CollectionAssert.IsEmpty(enumerator.Current);
		}

		[Test]
		public static void Constructor_WhenGetNextOptionsIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DFSEnumerator<int>(null));
		}
	}
}
