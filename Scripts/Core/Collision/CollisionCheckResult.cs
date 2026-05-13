
namespace JonathonOH.RoadGeneration.Collision
{
	public readonly struct CollisionCheckResult
	{
		public readonly CollisionCheckRequest Request { get; init; }
		public bool HasCollision { get; init; }
		public RoadSection CollidesWith { get; init; }
	}
}
