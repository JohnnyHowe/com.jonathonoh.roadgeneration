using System.Collections.Generic;
using System.Linq;

namespace JonathonOH.RoadGeneration.ChainGenerators
{
	public class DFSChainGenerator : IChainGenerator
	{
		private IReadOnlyList<IRoadSection> alreadyPlaced;
		private IReadOnlyList<IRoadSection> choicesInPreferenceOrder;

		public Chain Current => GetCurrentChain();
		private IEnumerable<IRoadSection> Candidates => GetCurrentCandidates();

		private List<int> searchStackCursor = new List<int>();

		public DFSChainGenerator(IReadOnlyList<IRoadSection> alreadyPlaced, IReadOnlyList<IRoadSection> choicesInPreferenceOrder)
		{
			this.alreadyPlaced = alreadyPlaced;
			this.choicesInPreferenceOrder = choicesInPreferenceOrder;
		}

		private Chain GetCurrentChain()
		{
			var candidatesList = Candidates.ToList();
			return new Chain()
			{
				AlreadyPlaced = alreadyPlaced,
				Candidates = candidatesList,
			};
		}

		private IEnumerable<IRoadSection> GetCurrentCandidates()
		{
			for (int stackIndex = 0; stackIndex < searchStackCursor.Count; stackIndex++)
			{
				int cursor = searchStackCursor[stackIndex];
				yield return choicesInPreferenceOrder[cursor];
			}
		}

		public void Extend()
		{
			searchStackCursor.Add(0);
		}

		public bool Next()
		{
			int currentDepth = searchStackCursor.Count - 1;
			searchStackCursor[currentDepth] += 1;

			if (searchStackCursor[currentDepth] < choicesInPreferenceOrder.Count)
			{
				return true;
			}
			else
			{
				return Backtrack();
			}
		}

		private bool Backtrack()
		{
			if (searchStackCursor.Count <= 1)
			{
				return false;
			}
			return Next();
		}
	}
}
