using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	internal static class PreciseVectorToString
	{
		public static string ToStringPrecise(this Vector2 v)
		{
			return $"Vector2({v.x}f, {v.y}f)";
		}
	}
}
