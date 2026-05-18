using System.Collections;
using System.Collections.Generic;

namespace JonathonOH.BacktrackableEnumerator
{
	public class DFSEnumerator<T> : IBacktrackableEnumerator<IReadOnlyList<T>>
	{
		public delegate IEnumerable<T> GetNextOptions(IReadOnlyList<T> current);
		private GetNextOptions getNextOptions;

		private Stack<IReadOnlyList<T>> stack;
		private Stack<int> stackState;

		public IReadOnlyList<T> Current => throw new System.NotImplementedException();

		object IEnumerator.Current => Current;

		public DFSEnumerator(GetNextOptions getNextOptions)
		{
			this.getNextOptions = getNextOptions;
			stack = new Stack<IReadOnlyList<T>>();

			stackState = new Stack<int>();
			stackState.Push(0);
		}

		public void Backtrack()
		{
			throw new System.NotImplementedException();
		}

		public bool MoveNext()
		{
			throw new System.NotImplementedException();
		}

		public void Reset()
		{
			throw new System.NotImplementedException();
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}
	}
}
