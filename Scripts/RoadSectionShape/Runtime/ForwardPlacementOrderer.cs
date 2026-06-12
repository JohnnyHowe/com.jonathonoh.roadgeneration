using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Core;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	public class ForwardPlacementOrderer
	{
		private struct SectionOrderEntry
		{
			public IRoadSection Section;
			public int OriginalIndex;
		}

		private ShapeCache shapeCache;
		private Dictionary<string, float> anglesCache;

		public ForwardPlacementOrderer(ShapeCache shapeCache)
		{
			this.shapeCache = shapeCache;
			anglesCache = new Dictionary<string, float>();
		}

		/// <summary>
		/// most straight, hardest left, hardest right, most straight, hardest left ...
		/// </summary>
		public IEnumerable<IRoadSection> GetOrdered(IEnumerable<IRoadSection> sections)
		{
			IList<SectionOrderEntry> sectionsByAngle = sections
				.Select((section, index) => new SectionOrderEntry
				{
					Section = section,
					OriginalIndex = index
				})
				.OrderBy(entry => GetAngleNormalized(entry.Section))
				.ToList();

			IList<SectionOrderEntry> leftTurnsOrderedByAngle = sectionsByAngle.Where(entry => GetSignedAngleNormalized(entry.Section) <= 0).Reverse().ToList();
			IList<SectionOrderEntry> rightTurnsOrderedByAngle = sectionsByAngle.Where(entry => GetSignedAngleNormalized(entry.Section) > 0).Reverse().ToList();

			HashSet<int> yielded = new HashSet<int>();
			int straightIndex = 0;
			int leftIndex = 0;
			int rightIndex = 0;

			while (yielded.Count < sectionsByAngle.Count)
			{
				if (TryGetNextUnique(sectionsByAngle, yielded, ref straightIndex, out IRoadSection straightSection))
				{
					yield return straightSection;
				}

				if (TryGetNextUnique(leftTurnsOrderedByAngle, yielded, ref leftIndex, out IRoadSection leftSection))
				{
					yield return leftSection;
				}

				if (TryGetNextUnique(rightTurnsOrderedByAngle, yielded, ref rightIndex, out IRoadSection rightSection))
				{
					yield return rightSection;
				}
			}
		}

		private static bool TryGetNextUnique(IList<SectionOrderEntry> sections, HashSet<int> yielded, ref int index, out IRoadSection section)
		{
			while (index < sections.Count)
			{
				SectionOrderEntry entry = sections[index];
				index++;

				if (yielded.Add(entry.OriginalIndex))
				{
					section = entry.Section;
					return true;
				}
			}

			section = null;
			return false;
		}

		/// <summary>
		/// </summary>

		private float GetAngleNormalized(IRoadSection section)
		{
			return Mathf.Abs(GetSignedAngleNormalized(section));
		}

		/// <summary>
		/// Higher value = likely more effective to check early.
		/// 
		/// Things taken into account:
		/// - angle difference between start and end (less is better)
		/// </summary>
		private float GetSignedAngleNormalized(IRoadSection section)
		{
			string key = section.GetShapeId();

			// Is the value cached?
			if (!anglesCache.ContainsKey(key))
			{
				RoadSectionShape shape = shapeCache.GetShape(section);
				anglesCache[key] = CalculateSignedAngleNormalized(shape);
			}

			return anglesCache[key];
		}

		private static float CalculateSignedAngleNormalized(RoadSectionShape shape)
		{
			float signedAngle = Vector3.SignedAngle(
				shape.Entry.rotation.eulerAngles,
				shape.Exit.rotation.eulerAngles,
				Vector3.up
			);
			float normalizedAngle = signedAngle / 180f;

			return normalizedAngle;
		}
	}
}
