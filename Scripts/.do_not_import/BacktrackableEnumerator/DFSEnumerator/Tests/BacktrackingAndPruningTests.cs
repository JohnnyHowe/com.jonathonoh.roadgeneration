using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JonathonOH.BacktrackableEnumerator.Tests
{
	public static class BacktrackingAndPruningTests
	{
		[Test]
		public static void Backtrack_WhenAtLeaf_MovesToNextSibling()
		{
			DFSEnumerator<string> enumerator = CreateEnumerator();
			MoveTo(enumerator, "A/B/D");

			Assert.IsTrue(enumerator.Backtrack());

			CollectionAssert.AreEqual(new[] { "A", "B", "E" }, enumerator.Current);
		}

		[Test]
		public static void Backtrack_WhenAtInternalNode_SkipsDescendants()
		{
			DFSEnumerator<string> enumerator = CreateEnumerator();
			MoveTo(enumerator, "A/B");

			Assert.IsTrue(enumerator.Backtrack());

			CollectionAssert.AreEqual(new[] { "A", "C" }, enumerator.Current);
		}

		[Test]
		public static void Backtrack_WhenNoSiblingExists_ClimbsToNearestAvailableSibling()
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
			MoveTo(enumerator, "A/B/D");

			Assert.IsTrue(enumerator.Backtrack());

			CollectionAssert.AreEqual(new[] { "A", "C" }, enumerator.Current);
		}

		[Test]
		public static void Backtrack_WhenAtRoot_ExhaustsTraversal()
		{
			DFSEnumerator<string> enumerator = CreateEnumerator();
			MoveTo(enumerator, "A");

			Assert.IsFalse(enumerator.Backtrack());
			Assert.IsFalse(enumerator.MoveNext());
		}

		[Test]
		public static void Backtrack_WhenTraversalIsAlreadyExhausted_ReturnsFalse()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				return current.Count == 0 ? new[] { "A" } : Array.Empty<string>();
			});
			Assert.IsTrue(enumerator.MoveNext());
			Assert.IsFalse(enumerator.MoveNext());

			Assert.IsFalse(enumerator.Backtrack());
		}

		[Test]
		public static void Backtrack_WhenCalledBeforeFirstMove_ReturnsFalseAndLeavesCurrentEmpty()
		{
			DFSEnumerator<string> enumerator = CreateEnumerator();

			Assert.IsFalse(enumerator.Backtrack());
			CollectionAssert.IsEmpty(enumerator.Current);
		}

		[Test]
		public static void Backtrack_WhenCandidateHasChildren_DoesNotVisitDescendants()
		{
			DFSEnumerator<string> enumerator = CreateEnumerator();
			MoveTo(enumerator, "A/B");

			Assert.IsTrue(enumerator.Backtrack());
			List<string> remainingPaths = CollectCurrentAndRemainingPaths(enumerator);

			CollectionAssert.AreEqual(new[] { "A/C", "A/C/F" }, remainingPaths);
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
						return new[] { "D", "E" };
					case "C":
						return new[] { "F" };
					default:
						return Array.Empty<string>();
				}
			});
		}

		private static void MoveTo(DFSEnumerator<string> enumerator, string expectedPath)
		{
			while (enumerator.MoveNext())
			{
				if (ToPathString(enumerator.Current) == expectedPath)
				{
					return;
				}
			}

			Assert.Fail("Expected path was not reached: " + expectedPath);
		}

		private static List<string> CollectCurrentAndRemainingPaths(DFSEnumerator<string> enumerator)
		{
			List<string> paths = new List<string> { ToPathString(enumerator.Current) };

			while (enumerator.MoveNext())
			{
				paths.Add(ToPathString(enumerator.Current));
			}

			return paths;
		}

		private static string ToPathString(IReadOnlyList<string> path)
		{
			return string.Join("/", path);
		}
	}
}
