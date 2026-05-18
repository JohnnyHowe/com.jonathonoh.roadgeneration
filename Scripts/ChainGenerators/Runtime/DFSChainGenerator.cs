using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JonathonOH.DFSChainGenerator;

namespace JonathonOH.RoadGeneration.ChainGenerators
{
	public class DFSChainGenerator : IChainGenerator
	{
		private IReadOnlyList<IRoadSection> alreadyPlaced;
		private IReadOnlyList<IRoadSection> choicesInPreferenceOrder;
		private int checkDepth;

		private Generator combinationGenerator;

		public Chain Current => GetCurrentChain();
		object IEnumerator.Current => Current;

		/// <summary>
		/// Store the complete set of road sections that the DFS search can choose from when building chains.
		/// </summary>
		public DFSChainGenerator(
			IReadOnlyList<IRoadSection> alreadyPlaced,
			IReadOnlyList<IRoadSection> choicesInPreferenceOrder,
			int checkDepth
		)
		{
			this.alreadyPlaced = alreadyPlaced;
			this.choicesInPreferenceOrder = choicesInPreferenceOrder;
			this.checkDepth = checkDepth;

			combinationGenerator = new Generator(choicesInPreferenceOrder.Count, checkDepth);
		}

		private Chain GetCurrentChain()
		{
			return new Chain()
			{
				AlreadyPlaced = alreadyPlaced,
				Candidates = GetCurrentCandidates().ToList(),
				// IsCandidatesAtTargetLength
			};
		}

		private IEnumerable<IRoadSection> GetCurrentCandidates()
		{
			foreach (int index in combinationGenerator.GetState())
			{
				if (index == -1)
				{
					yield break;
				}

				yield return choicesInPreferenceOrder[index];
			}
		}

		/// <summary>
		/// Release any search state owned by this generator when enumeration is finished.
		/// </summary>
		public void Dispose()
		{
			combinationGenerator.Reset();
		}

		/// <summary>
		/// Advance the DFS search to the next valid chain permutation and make it available through
		/// <see cref="Current"/>. Return false when no more valid chains can be produced.
		/// </summary>
		public bool MoveNext()
		{
			// combinationGenerator.StepValid();
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Advance the DFS search to the next invalid or rejected chain state so callers can inspect
		/// why a branch failed. Return false when there are no more invalid states to report.
		/// </summary>
		public bool MoveNextInvalid()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Rewind the DFS generator to its initial state so enumeration can start again from the full
		/// set of available sections.
		/// </summary>
		public void Reset()
		{
			throw new System.NotImplementedException();
		}
	}
}
