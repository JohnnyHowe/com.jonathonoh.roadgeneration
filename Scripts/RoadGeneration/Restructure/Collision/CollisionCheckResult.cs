
namespace JonathonOH.RoadGeneration.Collision
{
	public readonly struct CollisionCheckResult
	{
		public bool HasOverlap { get; init; }
		public RoadSection OverlapsWith { get; init; }
	}
}
