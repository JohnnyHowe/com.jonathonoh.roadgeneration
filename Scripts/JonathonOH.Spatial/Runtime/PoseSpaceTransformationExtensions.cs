using UnityEngine;

namespace JonathonOH.Spatial
{
	/// <summary>
	/// Use same convention as Unity's Transform.TransformPoint and InverseTransformPoint
	/// </summary>
	public static class PoseSpaceTransformationExtensions
	{
		/// <summary>
		/// Transforms localPose to world space relative to this.
		/// Mirrors Transform.TransformPoint.
		/// 
		/// Example:
		/// (Vector3(10, 0, 0), ...).TransformPoint((Vector3(1, 2, 0) ...)) -> (Vector3(11, 2, 0, ...))
		/// </summary>
		public static Pose TransformPose(this Pose pose, Pose localPose)
		{
			return localPose.GetTransformedBy(pose);
		}

		/// <summary>
		/// Transforms worldPose to local space relative to this.
		/// Mirrors Transform.InverseTransformPoint.
		/// 
		/// Example:
		/// (Vector3(10, 0, 0), ...).TransformPoint((Vector3(1, 2, 0) ...)) -> (Vector3(-9, -2, 0, ...))
		/// </summary>
		public static Pose InverseTransformPose(this Pose pose, Pose worldPose)
		{
			Quaternion inverseRotation = Quaternion.Inverse(pose.rotation);
			return new Pose(
				inverseRotation * (worldPose.position - pose.position),
				inverseRotation * worldPose.rotation);
		}
	}
}
