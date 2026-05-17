using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public static class ConvexHullTransformExtensions
	{
		/// <summary>
		/// Transforms this hull from local space into world space using the given pose.
		/// Uses horizontal pose components (x, z).
		/// </summary>
		public static ConvexHull TransformBy(this ConvexHull convexHull, Pose pose)
		{
			return new ConvexHull(TransformPoints(convexHull.Vertices, pose));
		}

		private static IEnumerable<Vector2> TransformPoints(IEnumerable<Vector2> points, Pose pose)
		{
			foreach (Vector2 point in points)
			{
				yield return TransformPoint(point, pose);
			}
		}

		private static Vector2 TransformPoint(Vector2 point, Pose pose)
		{
			Vector3 transformedPoint = pose.position + pose.rotation * new Vector3(point.x, 0f, point.y);
			return new Vector2(transformedPoint.x, transformedPoint.z);
		}

		/// <summary>
		/// Transforms this hull from world space into local space using the given pose.
		/// Uses horizontal pose components (x, z).
		/// </summary>
		public static ConvexHull InverseTransformBy(this ConvexHull convexHull, Pose pose)
		{
			return new ConvexHull(InverseTransformPoints(convexHull.Vertices, pose));
		}

		private static IEnumerable<Vector2> InverseTransformPoints(IEnumerable<Vector2> points, Pose pose)
		{
			foreach (Vector2 point in points)
			{
				yield return InverseTransformPoint(point, pose);
			}
		}

		private static Vector2 InverseTransformPoint(Vector2 point, Pose pose)
		{
			Quaternion inverseRotation = Quaternion.Inverse(pose.rotation);
			Vector3 transformedPoint = inverseRotation * (new Vector3(point.x, 0f, point.y) - pose.position);
			return new Vector2(transformedPoint.x, transformedPoint.z);
		}
	}
}
