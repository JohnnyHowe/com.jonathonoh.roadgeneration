using System.Collections.Generic;

namespace JonathonOH.BacktrackableEnumerator
{
	public interface IBacktrackableEnumerator<T>: IEnumerator<T>
	{
		public void Backtrack();
	}
}
