using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Core;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class ForwardPlacementOrderer
	{
		private ShapeCache shapeCache;
		private Dictionary<string, float> cache;

		public ForwardPlacementOrderer(ShapeCache shapeCache)
		{
			this.shapeCache = shapeCache;
			cache = new Dictionary<string, float>();
		}

		/// <summary>
		/// Higher heuristic first.
		/// </summary>
		public IEnumerable<IRoadSection> GetOrdered(IEnumerable<IRoadSection> sections)
		{
			return sections.OrderBy(GetHeuristic);
		}

		/// <summary>
		/// Higher value = likely more effective to check early.
		/// 
		/// Things taken into account:
		/// - angle difference between start and end (less is better)
		/// </summary>
		public float GetHeuristic(IRoadSection section)
		{
			string key = section.GetShapeId();

			// Is the value cached?
			if (!cache.ContainsKey(key))
			{
				RoadSectionShape shape = shapeCache.GetShape(section);
				cache[key] = CalculateHeuristic(shape);
			}

			return cache[key];
		}

		private static float CalculateHeuristic(RoadSectionShape shape)
		{
			return GetAngleNormalized(shape);
		}

		private static float GetAngleNormalized(RoadSectionShape shape)
		{
			// Quaternion.Angle returns [0, 180]
			// https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Quaternion.Angle.html
			float angle = Quaternion.Angle(shape.Entry.rotation, shape.Exit.rotation);
			float normalizedAngle = angle / 180f;

			return normalizedAngle;
		}
	}
}
