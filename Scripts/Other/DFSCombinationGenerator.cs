using System;
using System.Linq;
using UnityEngine;

namespace Other
{
    /// <summary>
    /// The essence of DFS traversal for a fixed number of branches at every node.
    /// With the depth and number of trees, it will keep track of the branch index at every depth
    ///  and allow stepping through it ez pz
    /// </summary>
    public class DFSCombinationGenerator : IDFSCombinationGenerator
    {
        public class OutOfCombinationsException : Exception { }

        private int[] _state;
        private int _maxDepth;
        private int _nBranches;
        private const int MAX_ITERATIONS = 10000;
        private bool _atSolution;
        private bool _exhaustedSearch;

        public DFSCombinationGenerator(int branches, int depth)
        {
            if (branches < 1) throw new ArgumentOutOfRangeException("Cannot have less than 1 branches");
            if (depth < 1) throw new ArgumentOutOfRangeException("Cannot have less than one depth");

            _maxDepth = depth;
            _nBranches = branches;
            _atSolution = false;

            SetState(Enumerable.Range(0, depth).Select(_ => -1).ToArray()); // init to { -1, -1, ...}
            _state[0] = 0;
            _exhaustedSearch = false;
        }

        public int[] GetState()
        {
            return _state;
        }

        public void SetState(int[] newState)
        {
            _state = newState;
        }

        public void StepInvalid()
        {
            _atSolution = false;

            int currentDepth = GetCurrentDepth();
            int indexInQuestion = currentDepth;

            if (_state[indexInQuestion] < _nBranches - 1)
            {
                // no backtrack
                _state[indexInQuestion]++;
            }
            else
            {
                _Backtrack();
            }
        }

        private void _Backtrack()
        {
            int currentDepth = GetCurrentDepth();
            int indexInQuestion = Mathf.Max(0, currentDepth);
            _state[indexInQuestion]++;

            // Acting as a while loop - I'm sick of breaking Unity through infinite loops 
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                // exit clause, imagine the contents in while(!...)
                if (_state[indexInQuestion] < _nBranches) break;

                // Cannot backtrack anymore
                if (!_CanBacktrack())
                {
                    _exhaustedSearch = true;
                    throw new OutOfCombinationsException();
                }

                // do backtrack
                _state[indexInQuestion] = -1;
                _state[indexInQuestion - 1]++;
                indexInQuestion -= 1;
            }
        }

        private bool _CanBacktrack()
        {
            int indexInQuestion = Mathf.Max(0, GetCurrentDepth());
            return indexInQuestion > 0;
        }

        public void StepValid()
        {
            if (_state[_maxDepth - 1] != -1)
            {
                _atSolution = true;
                return;
            }

            _state[GetCurrentDepth() + 1] = 0;
        }

        public bool HasFoundSolution()
        {
            return _atSolution;
        }

        public int GetCurrentDepth()
        {
            int firstNegativeOneIndex = Array.IndexOf(_state, -1);
            if (firstNegativeOneIndex >= 0) return firstNegativeOneIndex - 1;
            return _maxDepth - 1;
        }

        public int GetCurrentEnd()
        {
            return _state[Mathf.Max(0, GetCurrentDepth())];
        }

        public bool IsImpossible()
        {
            return _exhaustedSearch;
        }
    }
}