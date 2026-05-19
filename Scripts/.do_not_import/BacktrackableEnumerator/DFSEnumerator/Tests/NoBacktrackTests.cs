using System.Collections.Generic;
using NUnit.Framework;

namespace JonathonOH.BacktrackableEnumerator.Tests
{
	public static class NoBacktrackTests
	{
		[Test]
		public static void OneBranch_FiveDeep_ReturnsFivePermutations()
		{
			DFSEnumerator<int>.GetNextOptions getNextOptions = (IReadOnlyList<int> current) =>
			{
				if (current.Count >= 5)
				{
					return new int[] { };
				}
				return new[] { 0 };
			};

			DFSEnumerator<int> enumerator = new DFSEnumerator<int>(getNextOptions);
			List<IReadOnlyList<int>> allPermutations = ToList(enumerator);

			Assert.AreEqual(allPermutations.Count, 5);
		}

		[Test]
		public static void TwoBranches_TwoDeep_ReturnsSixPermutations()
		{
			DFSEnumerator<int>.GetNextOptions getNextOptions = (IReadOnlyList<int> current) =>
			{
				if (current.Count >= 2)
				{
					return new int[] { };
				}
				return new[] { 0, 1 };
			};

			DFSEnumerator<int> enumerator = new DFSEnumerator<int>(getNextOptions);
			List<IReadOnlyList<int>> allPermutations = ToList(enumerator);

			Assert.AreEqual(allPermutations.Count, 6);
		}

		private static List<T> ToList<T>(IEnumerator<T> enumerator)
		{
			List<T> list = new();

			while (enumerator.MoveNext())
			{
				list.Add(enumerator.Current);
			}

			return list;
		}
	}
}
