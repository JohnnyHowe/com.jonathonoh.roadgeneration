using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	internal class ShapeCache
	{
		private Dictionary<string, RoadSectionShape> cache;

		public ShapeCache()
		{
			cache = new Dictionary<string, RoadSectionShape>();
		}

		public RoadSectionShape GetShape(IRoadSectionShapeCollisionCheckable section)
		{
			string id = section.GetId();

			if (!cache.ContainsKey(id))
			{
				cache[id] = section.GetRoadSectionShape();
				// Add temp debug log here to ensure we're only getting the bits we need
			}

			return cache[id];
		}
	}
}
