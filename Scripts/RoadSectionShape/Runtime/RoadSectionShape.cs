using JonathonOH.ConvexShapeExtruded;
using JonathonOH.RoadGeneration.Core;
using JonathonOH.Spatial;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	/// <summary>
	/// Describes the shape of a road section
	/// Contains logic for bounding areas, and start and end position alignment.
	/// 
	/// TODO make readonly
	/// </summary>
	public class RoadSectionShape
	{
		public readonly Pose Entry;
		public readonly Pose Exit;
		public readonly ConvexHullExtruded Hull;

		#region Constructors

		public static RoadSectionShape FromRoadSection(IRoadSection roadSection)
		{
			return FromMesh(
				roadSection.Entry,
				roadSection.Exit,
				roadSection.GetBoundaryInEntrySpace()
			);
		}

		public static RoadSectionShape FromMesh(Pose entry, Pose exit, Mesh meshBoundaryRelativetoEntry)
		{
			return new RoadSectionShape(entry, exit, ConvexHullExtruded.FromMesh(meshBoundaryRelativetoEntry));
		}

		public RoadSectionShape(Pose entry, Pose exit, ConvexHullExtruded hull)
		{
			Entry = entry;
			Exit = exit;
			Hull = hull;
		}

		#endregion

		public RoadSectionShape GetTranslatedCopy(Pose newEntry)
		{
			Pose originalExitRelativeToOriginalEntry = Entry.InverseTransformPose(Exit);
			Pose newExit = newEntry.TransformPose(originalExitRelativeToOriginalEntry);

			ConvexHullExtruded hullRelativeToEntry = Hull.InverseTransformBy(Entry);
			ConvexHullExtruded newHull = hullRelativeToEntry.TransformBy(newEntry);

			return new RoadSectionShape
			(
				newEntry,
				newExit,
				newHull
			);
		}

		public bool OverlapsWith(RoadSectionShape other)
		{
			return Hull.OverlapsWith(other.Hull);
		}

		#region Debug

		public void DebugDraw()
		{
			DebugDraw(Color.red);
		}

		public void DebugDraw(Color color)
		{
#if UNITY_EDITOR
			Entry.DebugDraw();
			Exit.DebugDraw();
			Debug.DrawLine(Entry.position, Exit.position, color);
			Hull.DebugDraw(color);
#endif
		}
		#endregion
	}
}
