using System.Collections.Generic;
using JonathonOH.RoadGeneration.Core;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class ShapeCache
	{
		private Dictionary<string, RoadSectionShape> cache;

		public ShapeCache()
		{
			cache = new Dictionary<string, RoadSectionShape>();
		}

		public Dictionary<IRoadSection, RoadSectionShape> GetShapesMapped(IEnumerable<IRoadSection> sections)
		{
			var mapping = new Dictionary<IRoadSection, RoadSectionShape>();
			foreach (IRoadSection section in sections)
			{
				mapping[section] = GetShape(section);
			}
			return mapping;
		}

		public IEnumerable<RoadSectionShape> GetShapes(IEnumerable<IRoadSection> sections)
		{
			foreach (IRoadSection section in sections)
			{
				yield return GetShape(section);
			}
		}

		public RoadSectionShape GetShape(IRoadSection section)
		{
			string id = section.GetShapeId();

			if (!IsCachable(id))
			{
				return GetSectionShape(section);
			}

			Add(id, section);
			return cache[id];
		}

		public void Add(IEnumerable<IRoadSection> sections)
		{
			foreach (var section in sections)
			{
				Add(section);
			}
		}

		public void Add(IRoadSection section)
		{
			Add(section.GetShapeId(), section);
		}

		public void Add(string id, IRoadSection section)
		{
			// If id was not set for whatever reason just skip.
			if (!IsCachable(id))
			{
				return;
			}

			if (!cache.ContainsKey(id))
			{
				cache[id] = GetSectionShape(section);
				// Add temp debug log here to ensure we're only getting the bits we need
			}
		}

		private bool IsCachable(string id)
		{
			return id.Trim() != "";
		}

		private RoadSectionShape GetSectionShape(IRoadSection roadSection)
		{
			return RoadSectionShape.FromRoadSection(roadSection);
		}
	}
}
