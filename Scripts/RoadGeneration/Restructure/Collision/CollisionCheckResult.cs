
namespace JonathonOH.RoadGeneration.Collision
{
	public readonly struct CollisionCheckResult
	{
		public bool HasCollision { get; init; }
		public RoadSection Subject { get; init; }
		public RoadSection CollidesWith { get; init; }

		public static CollisionCheckResult CreateWithoutCollision(RoadSection subject)
		{
			return new CollisionCheckResult()
			{
				HasCollision = false,
				Subject = subject,
				CollidesWith = null
			};
		}

		public static CollisionCheckResult CreateWithCollision(RoadSection subject, RoadSection collidesWith)
		{
			return new CollisionCheckResult()
			{
				HasCollision = true,
				Subject = subject,
				CollidesWith = collidesWith
			};
		}
	}
}
