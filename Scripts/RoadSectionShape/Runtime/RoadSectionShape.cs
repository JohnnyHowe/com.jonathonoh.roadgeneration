using System;
using System.Collections.Generic;
using JonathonOH.RoadGeneration.ConvexShape2D;
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
		public readonly FloatRange VerticalRange;
		public readonly ConvexHull HorizontalHull;
		public readonly bool InfiniteHeight;

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
			ConvexHull horizontalHull = ConvexHullConstructors.FromMesh(meshBoundaryRelativetoEntry, Vector3.up);
			return new RoadSectionShape(entry, exit, horizontalHull, null);
		}

		public RoadSectionShape(Pose entry, Pose exit, ConvexHull hull, FloatRange? verticalRange)
		{
			Entry = entry;
			Exit = exit;
			HorizontalHull = hull;
			InfiniteHeight = verticalRange == null;

			if (verticalRange != null)
			{
				InfiniteHeight = false;
				VerticalRange = (FloatRange)verticalRange;
			}
			else
			{
				InfiniteHeight = true;
			}
		}

		#endregion

		public RoadSectionShape GetTranslatedCopy(Pose newEntry)
		{
			Pose originalExitRelativeToOriginalEntry = Entry.InverseTransformPose(Exit);
			Pose newExit = newEntry.TransformPose(originalExitRelativeToOriginalEntry);

			ConvexHull newHull = HorizontalHull.InverseTransformBy(Entry);

			return new RoadSectionShape
			(
				newEntry,
				newExit,
				newHull,
				InfiniteHeight ? null : VerticalRange
			);
		}

		public bool DoesOverlapWith(RoadSectionShape other)
		{
			if (!OverlapsWithOnVerticalAxis(other))
			{
				return false;
			}
			return HorizontalHull.OverlapsWith(other.HorizontalHull);
		}

		private bool OverlapsWithOnVerticalAxis(RoadSectionShape other)
		{
			if (InfiniteHeight || other.InfiniteHeight)
			{
				return true;
			}
			return VerticalRange.OverlapsWith(other.VerticalRange);
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

			IEnumerable<Vector2> topology = HorizontalHull.Vertices;
			foreach (Vector2 vertex1 in topology)
			{
				Debug.DrawLine(new Vector3(vertex1.x, VerticalRange.Min, vertex1.y), new Vector3(vertex1.x, VerticalRange.Max, vertex1.y), color);
				foreach (Vector2 vertex2 in topology)
				{
					Debug.DrawLine(new Vector3(vertex1.x, VerticalRange.Min, vertex1.y), new Vector3(vertex2.x, VerticalRange.Min, vertex2.y), color);
					Debug.DrawLine(new Vector3(vertex1.x, VerticalRange.Max, vertex1.y), new Vector3(vertex2.x, VerticalRange.Max, vertex2.y), color);
				}
			}
#endif
		}
		#endregion
	}
}
