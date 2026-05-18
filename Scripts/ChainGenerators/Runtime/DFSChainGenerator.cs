using System.Collections;
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.ChainGenerators
{
	public class DFSChainGenerator : IChainGenerator
	{
		private IReadOnlyList<IRoadSection> availableSections;

		public DFSChainGenerator(IReadOnlyList<IRoadSection> availableSections)
		{
			this.availableSections = availableSections;
		}

		public Chain Current => throw new System.NotImplementedException();

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		public bool MoveNext()
		{
			throw new System.NotImplementedException();
		}

		public bool MoveNextInvalid()
		{
			throw new System.NotImplementedException();
		}

		public void Reset()
		{
			throw new System.NotImplementedException();
		}
	}
}
