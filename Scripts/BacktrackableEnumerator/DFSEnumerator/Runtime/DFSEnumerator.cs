using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JonathonOH.BacktrackableEnumerator
{
	public class DFSEnumerator<T> : IBacktrackableEnumerator<IReadOnlyList<T>>
	{
		public delegate IEnumerable<T> GetNextOptions(IReadOnlyList<T> current);
		private GetNextOptions getNextOptions;

		private List<IReadOnlyList<T>> stack;
		private List<int> stackCursor;

		private int currentStackDepthIndex => stackCursor.Count - 1;
		private int currentStackItemCursorIndex
		{
			get => stackCursor[currentStackDepthIndex];
			set { stackCursor[currentStackDepthIndex] = value; }
		}

		public IReadOnlyList<T> Current => GetCurrent().ToList();
		object IEnumerator.Current => Current;

		public DFSEnumerator(GetNextOptions getNextOptions)
		{
			if (getNextOptions == null)
			{
				throw new ArgumentNullException();
			}
			this.getNextOptions = getNextOptions;
			Reset();
		}

		public void Reset()
		{
			stackCursor = new List<int>();
			stack = new List<IReadOnlyList<T>>();
		}

		public bool Backtrack()
		{
			return MoveBack();
		}

		public bool MoveNext()
		{
			if (MoveDeeper())
			{
				return true;
			}
			if (MoveSideways())
			{
				return true;
			}
			if (MoveBack())
			{
				return true;
			}
			return false;
		}

		private bool MoveDeeper()
		{
			IEnumerable<T> nextOptions = getNextOptions.Invoke(Current);

			if (nextOptions == null)
			{
				throw new ArgumentNullException();
			}

			List<T> nextOptionsList = nextOptions.ToList();

			if (nextOptionsList.Count == 0)
			{
				return false;
			}

			stack.Add(nextOptionsList);
			stackCursor.Add(0);
			return true;
		}

		private bool MoveSideways()
		{
			if (stack.Count == 0)
			{
				return false;
			}

			IReadOnlyList<T> currentOptions = stack[currentStackDepthIndex];

			if (currentStackItemCursorIndex + 1 >= currentOptions.Count)
			{
				return false;
			}
			
			currentStackItemCursorIndex++;
			return true;
		}

		private bool MoveBack()
		{
			while (stack.Count > 1)
			{
				stack.RemoveAt(currentStackDepthIndex);
				stackCursor.RemoveAt(currentStackDepthIndex);

				if (MoveSideways())
				{
					return true;
				}
			}
			return false;
		}

		private IEnumerable<T> GetCurrent()
		{
			for (int i = 0; i < stackCursor.Count; i++)
			{
				int indexInState = stackCursor[i];
				IReadOnlyList<T> stackEntry = stack[i];
				yield return stackEntry[indexInState];
			}
		}

		public void Dispose() { }
	}
}
