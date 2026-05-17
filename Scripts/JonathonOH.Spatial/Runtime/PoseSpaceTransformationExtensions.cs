using System;
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Mirrors Transform.TransformPoint.
		/// https://docs.unity3d.com/ScriptReference/Transform.TransformPoint.html
		/// </summary>
		public static Vector3 TransformPoint(this Pose pose, Vector3 point)
		{
			throw new NotImplementedException();
		}
	}
}
