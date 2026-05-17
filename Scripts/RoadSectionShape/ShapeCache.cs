using System.Collections.Generic;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class ShapeCache
	{
		private Dictionary<string, RoadSectionShape> cache;

		public ShapeCache()
		{
			cache = new Dictionary<string, RoadSectionShape>();
		}

		public Dictionary<IRoadSectionShapeCollisionCheckable, RoadSectionShape> GetShapesMapped(IEnumerable<IRoadSectionShapeCollisionCheckable> sections)
		{
			var mapping = new Dictionary<IRoadSectionShapeCollisionCheckable, RoadSectionShape>();
			foreach (IRoadSectionShapeCollisionCheckable section in sections)
			{
				mapping[section] = GetShape(section);
			}
			return mapping;
		}

		public IEnumerable<RoadSectionShape> GetShapes(IEnumerable<IRoadSectionShapeCollisionCheckable> sections)
		{
			foreach (IRoadSectionShapeCollisionCheckable section in sections)
			{
				yield return GetShape(section);
			}
		}

		public RoadSectionShape GetShape(IRoadSectionShapeCollisionCheckable section)
		{
			string id = section.GetId();

			if (!IsCachable(id))
			{
				return section.GetRoadSectionShape();
			}

			Add(id, section);
			return cache[id];
		}

		public void Add(IEnumerable<IRoadSectionShapeCollisionCheckable> sections)
		{
			foreach (var section in sections)
			{
				Add(section);
			}
		}

		public void Add(IRoadSectionShapeCollisionCheckable section)
		{
			Add(section.GetId(), section);
		}

		public void Add(string id, IRoadSectionShapeCollisionCheckable section)
		{
			// If id was not set for whatever reason just skip.
			if (!IsCachable(id))
			{
				return;
			}

			if (!cache.ContainsKey(id))
			{
				cache[id] = section.GetRoadSectionShape();
				// Add temp debug log here to ensure we're only getting the bits we need
			}
		}

		private bool IsCachable(string id)
		{
			return id.Trim() != "";
		}
	}
}
