using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JonathonOH.BacktrackableEnumerator.Tests
{
	public static class BasicDFSTests
	{
		[Test]
		public static void MoveNext_WhenSingleRoot_EmitsRootThenCompletes()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				return current.Count == 0 ? new[] { "A" } : Array.Empty<string>();
			});

			Assert.IsTrue(enumerator.MoveNext());
			CollectionAssert.AreEqual(new[] { "A" }, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());
		}

		[Test]
		public static void MoveNext_WhenFlatRootHasMultipleChildren_EmitsChildrenInDeclaredOrder()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				return current.Count == 0 ? new[] { "A", "B", "C" } : Array.Empty<string>();
			});

			List<string> paths = ToPathStrings(enumerator);

			CollectionAssert.AreEqual(new[] { "A", "B", "C" }, paths);
		}

		[Test]
		public static void MoveNext_WhenBalancedTree_EmitsPreorderDepthFirstSequence()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				if (current.Count == 0)
				{
					return new[] { "A" };
				}

				string node = current[current.Count - 1];
				switch (node)
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

			List<string> paths = ToPathStrings(enumerator);

			CollectionAssert.AreEqual(
				new[]
				{
					"A",
					"A/B",
					"A/B/D",
					"A/B/E",
					"A/C",
					"A/C/F"
				},
				paths
			);
		}

		[Test]
		public static void MoveNext_WhenUnbalancedTree_VisitsDeepBranchBeforeSibling()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				if (current.Count == 0)
				{
					return new[] { "A" };
				}

				string node = current[current.Count - 1];
				switch (node)
				{
					case "A":
						return new[] { "B", "C" };
					case "B":
						return new[] { "D" };
					case "D":
						return new[] { "E" };
					case "C":
						return new[] { "F" };
					default:
						return Array.Empty<string>();
				}
			});

			List<string> paths = ToPathStrings(enumerator);

			CollectionAssert.AreEqual(
				new[]
				{
					"A",
					"A/B",
					"A/B/D",
					"A/B/D/E",
					"A/C",
					"A/C/F"
				},
				paths
			);
		}

		[Test]
		public static void MoveNext_AfterExhaustion_RemainsExhausted()
		{
			DFSEnumerator<string> enumerator = new DFSEnumerator<string>((IReadOnlyList<string> current) =>
			{
				return current.Count == 0 ? new[] { "A" } : Array.Empty<string>();
			});

			Assert.IsTrue(enumerator.MoveNext());
			Assert.IsFalse(enumerator.MoveNext());
			Assert.IsFalse(enumerator.MoveNext());
		}

		private static List<string> ToPathStrings(IEnumerator<IReadOnlyList<string>> enumerator)
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
