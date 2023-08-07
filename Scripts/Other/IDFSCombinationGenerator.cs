namespace Other
{
    public interface IDFSCombinationGenerator
    {
        /// <summary>
        /// The state of the DFS traversal.
        /// An array of integers, containing the indices of the branches chosen at each depth
        /// </summary>
        int[] GetState();

        /// <summary>
        /// Set the internal state of the DFS traversal.
        /// See GetState for what the state means.
        /// </summary>
        void SetState(int[] newState);

        void StepInvalid();
        void StepValid();

        int GetCurrentDepth();
        int GetCurrentEnd();

        bool HasFoundSolution();
    }
}