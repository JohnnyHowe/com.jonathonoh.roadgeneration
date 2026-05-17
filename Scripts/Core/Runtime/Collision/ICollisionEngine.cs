namespace JonathonOH.RoadGeneration.Collision
{
	public interface ICollisionEngine
	{
		public enum SearchResult
		{
			SolutionFound,
			Impossible,
			SearchNotFinished
		}

		public void Reset(CollisionCheckRequest request);
		public void Step();
		public SearchResult GetResult();
	}
}
