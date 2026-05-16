using System;
using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	[Serializable]
	internal class ShapeCache
	{
		private Dictionary<string, RoadSectionShape> cache;

		public ShapeCache()
		{
			cache = new Dictionary<string, RoadSectionShape>();
		}

		public RoadSectionShape GetShape(IRoadSectionShapeCollisionCheckable section)
		{
			throw new NotImplementedException();
		}
	}
}
