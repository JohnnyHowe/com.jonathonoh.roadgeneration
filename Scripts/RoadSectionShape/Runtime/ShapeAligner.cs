using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	/// <summary>
	/// Utility for aligning RoadSectionShape objects
	/// </summary>
	public static class ShapeAligner
	{
		public static readonly Pose DefaultStart = new Pose(Vector3.zero, Quaternion.identity);

		/// <summary>
		/// Returns a new enumerable of all the shape parameters aligned in one chain.
		/// If fixedShapes is empty, the chain Start is the origin (Position=Vector3.zero, Rotation=Quaternion.Identity).
		/// Order is order of inputs: [**fixedShapes, **shapesToAlign].
		/// </summary>
		public static IEnumerable<RoadSectionShape> GetAligned(IReadOnlyList<RoadSectionShape> fixedShapes, IEnumerable<RoadSectionShape> shapesToAlign)
		{
			Pose alignmentStart = GetLastShapeEndOrDefault(fixedShapes);
			IEnumerable<RoadSectionShape> aligned = GetAligned(alignmentStart, shapesToAlign);
			return fixedShapes.Concat(aligned);
		}

		private static Pose GetLastShapeEndOrDefault(IReadOnlyList<RoadSectionShape> fixedShapes)
		{
			if (fixedShapes.Count > 0)
			{
				return fixedShapes.Last().End;
			}
			return DefaultStart;
		}

		/// <summary>
		/// Returns a new enumerable of all the shape parameters aligned in one chain.
		/// </summary>
		public static IEnumerable<RoadSectionShape> GetAligned(IEnumerable<RoadSectionShape> shapesToAlign)
		{
			return GetAligned(DefaultStart, shapesToAlign);
		}
	
		/// <summary>
		/// Returns a new enumerable of all the shape parameters aligned in one chain.
		/// </summary>
		public static IEnumerable<RoadSectionShape> GetAligned(Pose start, IEnumerable<RoadSectionShape> shapesToAlign)
		{
			Pose nextStart = start;
			foreach (RoadSectionShape shapeToAlign in shapesToAlign)
			{
				RoadSectionShape aligned = GetAligned(nextStart, shapeToAlign);
				nextStart = aligned.End;
				yield return aligned;
			}
		}

		/// <summary>
		/// Returns an aligned RoadSectionShape.
		/// </summary>
		public static RoadSectionShape GetAligned(Pose start, RoadSectionShape shapeToMove)
		{
			return shapeToMove.GetTranslatedCopy(start);
		}
	}
}
