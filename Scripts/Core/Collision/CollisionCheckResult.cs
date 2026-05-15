
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.Collision
{
	public readonly struct CollisionCheckResult
	{
		public readonly CollisionCheckRequest Request { get; init; }
		public bool HasCollision { get; init; }
		public RoadSection CollidesWith { get; init; }

		public override string ToString()
		{
			List<string> contents = new List<string>() { $"HasCollision={HasCollision}" };
			if (HasCollision)
			{
				contents.Add($"CollidesWith={CollidesWith.gameObject.name}");
			}
			contents.Add($"Request={Request}");

			return $"CollisionCheckResult<{string.Join(", ", contents)}>";
		}
	}
}
