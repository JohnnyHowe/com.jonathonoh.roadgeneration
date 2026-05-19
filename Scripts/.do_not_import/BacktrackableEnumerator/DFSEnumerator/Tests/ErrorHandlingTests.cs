using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JonathonOH.BacktrackableEnumerator.Tests
{
	public static class ErrorHandlingTests
	{
		[Test]
		public static void MoveNext_WhenGetNextOptionsReturnsNull_ThrowsArgumentNullException()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) => null);

			Assert.Throws<ArgumentNullException>(() => enumerator.MoveNext());
		}

		[Test]
		public static void MoveNext_WhenGetNextOptionsThrows_PropagatesException()
		{
			InvalidOperationException expectedException = new InvalidOperationException("Traversal failed.");
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				throw expectedException;
			});

			InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());

			Assert.AreSame(expectedException, actualException);
		}

		[Test]
		public static void MoveNext_WhenGetNextOptionsThrows_DoesNotAdvanceCurrent()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				throw new InvalidOperationException("Traversal failed.");
			});

			Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());

			CollectionAssert.IsEmpty(enumerator.Current);
		}
	}
}
