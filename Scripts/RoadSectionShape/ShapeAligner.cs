
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
		private static readonly TransformData defaultStart = new TransformData(Vector3.zero, Quaternion.identity, Vector3.one);

		/// <summary>
		/// Returns a new enumerable of all the shape parameters aligned in one chain.
		/// If fixedShapes is empty, the chain Start is the origin (Position=Vector3.zero, Rotation=Quaternion.Identity).
		/// Order is order of inputs: [**fixedShapes, **shapesToAlign].
		/// </summary>
		public static IEnumerable<RoadSectionShape> GetAllAligned(IReadOnlyList<RoadSectionShape> fixedShapes, IEnumerable<RoadSectionShape> shapesToAlign)
		{
			TransformData alignmentStart = GetLastShapeEndOrDefault(fixedShapes);
			IEnumerable<RoadSectionShape> aligned = GetAllAligned(alignmentStart, shapesToAlign);
			return fixedShapes.Concat(aligned);
		}

		private static TransformData GetLastShapeEndOrDefault(IReadOnlyList<RoadSectionShape> fixedShapes)
		{
			if (fixedShapes.Count > 0)
			{
				return fixedShapes.Last().End;
			}
			return defaultStart;
		}

		/// <summary>
		/// Returns a new enumerable of all the shape parameters aligned in one chain.
		/// </summary>
		public static IEnumerable<RoadSectionShape> GetAllAligned(TransformData start, IEnumerable<RoadSectionShape> shapesToAlign)
		{
			TransformData nextStart = start;
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
		public static RoadSectionShape GetAligned(TransformData start, RoadSectionShape shapeToMove)
		{
			return shapeToMove.GetTranslatedCopy(start);
		}
	}
}
