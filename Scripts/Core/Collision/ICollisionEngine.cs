namespace JonathonOH.RoadGeneration.Collision
{
	public interface ICollisionEngine
	{
		public void Reset(CollisionCheckRequestNew request);
		public void Step();
		public CollisionCheckResult? GetResult();
	}
}
