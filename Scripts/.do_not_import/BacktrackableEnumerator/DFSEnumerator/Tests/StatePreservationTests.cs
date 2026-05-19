using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JonathonOH.BacktrackableEnumerator.Tests
{
	public static class StatePreservationTests
	{
		[Test]
		public static void MoveNext_WhenCalledRepeatedly_AdvancesOneStepAtATime()
		{
			DFSEnumerator<string> enumerator = CreateEnumerator();

			Assert.IsTrue(enumerator.MoveNext());
			CollectionAssert.AreEqual(new[] { "A" }, enumerator.Current);

			Assert.IsTrue(enumerator.MoveNext());
			CollectionAssert.AreEqual(new[] { "A", "B" }, enumerator.Current);

			Assert.IsTrue(enumerator.MoveNext());
			CollectionAssert.AreEqual(new[] { "A", "B", "D" }, enumerator.Current);
		}

		[Test]
		public static void Current_BeforeNextSuccessfulMove_RemainsStable()
		{
			DFSEnumerator<string> enumerator = CreateEnumerator();

			Assert.IsTrue(enumerator.MoveNext());
			IReadOnlyList<string> firstRead = enumerator.Current;
			IReadOnlyList<string> secondRead = enumerator.Current;

			CollectionAssert.AreEqual(firstRead, secondRead);
		}

		[Test]
		public static void MoveNext_AfterExhaustion_DoesNotMutateLastValidCurrent()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				return current.Count == 0 ? new[] { "A" } : Array.Empty<string>();
			});

			Assert.IsTrue(enumerator.MoveNext());
			IReadOnlyList<string> lastValidCurrent = enumerator.Current;

			Assert.IsFalse(enumerator.MoveNext());
			CollectionAssert.AreEqual(lastValidCurrent, enumerator.Current);
		}

		[Test]
		public static void MoveNext_WhenTwoEnumeratorsUseSameGraph_DoesNotShareTraversalState()
		{
			DFSEnumerator<string> first = CreateEnumerator();
			DFSEnumerator<string> second = CreateEnumerator();

			Assert.IsTrue(first.MoveNext());
			Assert.IsTrue(first.MoveNext());
			Assert.IsTrue(second.MoveNext());

			CollectionAssert.AreEqual(new[] { "A", "B" }, first.Current);
			CollectionAssert.AreEqual(new[] { "A" }, second.Current);
		}

		private static DFSEnumerator<string> CreateEnumerator()
		{
			return new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				if (current.Count == 0)
				{
					return new[] { "A" };
				}

				switch (current[current.Count - 1])
				{
					case "A":
						return new[] { "B", "C" };
					case "B":
						return new[] { "D" };
					default:
						return Array.Empty<string>();
				}
			});
		}
	}
}
