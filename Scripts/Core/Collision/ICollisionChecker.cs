namespace JonathonOH.RoadGeneration.Collision
{
	public interface ICollisionChecker
	{
		public CollisionCheckResult CheckOneAgainstMany(CollisionCheckRequest request);
	}
}
