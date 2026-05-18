using JonathonOH.RoadGeneration.ConvexShape2D;
using UnityEngine;

namespace JonathonOH.ConvexShapeExtruded
{
	public static class ConvexHullExtrudedTransformExtensions
	{
		private const float almostZero = 0.0001f;

		public static ConvexHullExtruded TransformBy(this ConvexHullExtruded target, Pose pose)
		{
			pose = EnsureOnlyRotationExistsOnYAxis(pose);

			ConvexHull horizontalHull = target.HorizontalHull.TransformBy(pose);
			FloatRange verticalRange = new FloatRange(target.VerticalRange.Min + pose.position.y, target.VerticalRange.Max + pose.position.y);
			return new ConvexHullExtruded(horizontalHull, verticalRange);
		}

		public static ConvexHullExtruded InverseTransformBy(this ConvexHullExtruded target, Pose pose)
		{
			pose = EnsureOnlyRotationExistsOnYAxis(pose);

			ConvexHull horizontalHull = target.HorizontalHull.InverseTransformBy(pose);
			FloatRange verticalRange = new FloatRange(target.VerticalRange.Min - pose.position.y, target.VerticalRange.Max - pose.position.y);
			return new ConvexHullExtruded(horizontalHull, verticalRange);
		}

		private static Pose EnsureOnlyRotationExistsOnYAxis(Pose pose)
		{
			return new Pose(pose.position, EnsureOnlyRotationExistsOnYAxis(pose.rotation));
		}

		private static Quaternion EnsureOnlyRotationExistsOnYAxis(Quaternion rotation)
		{
			return Quaternion.Euler(EnsureOnlyRotationExistsOnYAxis(rotation.eulerAngles));
		}

		private static Vector3 EnsureOnlyRotationExistsOnYAxis(Vector3 eulerAngles)
		{
			if (Mathf.Abs(eulerAngles.x) > almostZero || Mathf.Abs(eulerAngles.z) > almostZero)
			{
				Debug.LogError($"Rotating on x or z axis not supported! Got {eulerAngles} euler angles.");
			}
			return new Vector3(0, eulerAngles.y, 0);
		}
	}
}
