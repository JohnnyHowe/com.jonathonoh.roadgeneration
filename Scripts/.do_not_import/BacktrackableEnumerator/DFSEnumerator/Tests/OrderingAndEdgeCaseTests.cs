using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JonathonOH.BacktrackableEnumerator.Tests
{
	public static class OrderingAndEdgeCaseTests
	{
		[Test]
		public static void MoveNext_WhenChildrenAreProvidedInSpecificOrder_ExploresThatOrder()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				if (current.Count == 0)
				{
					return new[] { "Root" };
				}

				return current[current.Count - 1] == "Root" ? new[] { "Third", "First", "Second" } : Array.Empty<string>();
			});

			CollectionAssert.AreEqual(new[] { "Root", "Root/Third", "Root/First", "Root/Second" }, ToPathStrings(enumerator));
		}

		[Test]
		public static void MoveNext_AfterSubtreeIsExhausted_ContinuesWithNearestUnvisitedSibling()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
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

			CollectionAssert.AreEqual(new[] { "A", "A/B", "A/B/D", "A/C" }, ToPathStrings(enumerator));
		}

		[Test]
		public static void MoveNext_WhenEmptyChildListIsReturned_TreatsCurrentPathAsLeaf()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				return current.Count == 0 ? new[] { "A" } : new string[] { };
			});

			CollectionAssert.AreEqual(new[] { "A" }, ToPathStrings(enumerator));
		}

		[Test]
		public static void MoveNext_WhenDuplicateValuesAreDistinctOptions_EmitsEachOption()
		{
			DFSEnumerator<int> enumerator = new DFSEnumerator<int>((IReadOnlyList<int> current) =>
			{
				return current.Count == 0 ? new[] { 7, 7 } : Array.Empty<int>();
			});
			int emittedCount = 0;

			while (enumerator.MoveNext())
			{
				emittedCount++;
				CollectionAssert.AreEqual(new[] { 7 }, enumerator.Current);
			}

			Assert.AreEqual(2, emittedCount);
		}

		[Test]
		public static void MoveNext_WhenGraphIsDeepChain_PreservesDepthOrder()
		{
			const int maxDepth = 8;
			DFSEnumerator<int> enumerator = new DFSEnumerator<int>((IReadOnlyList<int> current) =>
			{
				return current.Count < maxDepth ? new[] { current.Count + 1 } : Array.Empty<int>();
			});

			Assert.IsTrue(enumerator.MoveNext());
			for (int i = 2; i <= maxDepth; i++)
			{
				Assert.IsTrue(enumerator.MoveNext());
			}

			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());
		}

		[Test]
		public static void MoveNext_WhenGraphHasWideSiblingSet_VisitsEverySiblingOnce()
		{
			DFSEnumerator<int> enumerator = new DFSEnumerator<int>((IReadOnlyList<int> current) =>
			{
				return current.Count == 0 ? new[] { 1, 2, 3, 4, 5 } : Array.Empty<int>();
			});

			CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "5" }, ToPathStrings(enumerator));
		}

		private static List<string> ToPathStrings<T>(IEnumerator<IReadOnlyList<T>> enumerator)
		{
			List<string> paths = new();

			while (enumerator.MoveNext())
			{
				paths.Add(string.Join("/", enumerator.Current));
			}

			return paths;
		}
	}
}
