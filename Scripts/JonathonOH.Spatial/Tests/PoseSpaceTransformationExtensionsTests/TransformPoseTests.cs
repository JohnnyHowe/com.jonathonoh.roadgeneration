using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Spatial.Tests.TransformPoseTests
{
	public static class TransformPoseTests
	{
		// Test cases to create for Pose.TransformPose
		// - Identity parent pose returns the local pose unchanged.
		// - Identity local pose returns the parent pose unchanged.
		// - Parent translation with identity rotations offsets local position by parent position.
		// - Parent rotation with zero parent translation rotates the local position into world space.
		// - Parent translation and rotation both apply to the local position:
		//   worldPosition = parent.position + parent.rotation * local.position.
		// - Local rotation is composed after parent rotation:
		//   worldRotation = parent.rotation * local.rotation.
		// - Non-axis-aligned rotations compose correctly, not just 90 degree yaw cases.
		// - Negative local positions are transformed correctly.
		// - TransformPose followed by InverseTransformPose returns the original local pose.
		// - Result matches a Unity Transform configured with the same parent pose and local pose.

		[Test]
		public static void IdentityParentPose_ReturnsLocalPoseUnchanged()
		{
			Pose parentPose = Pose.identity;
			Pose localPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			Pose expected = localPose;
			RunTest(parentPose, localPose, expected);
		}

		private static void RunTest(Pose thisPose, Pose localPose, Pose expected)
		{
			var result = thisPose.TransformPose(localPose);
			Assert.AreEqual(expected.position, result.position);
			Assert.That(Quaternion.Angle(expected.rotation, result.rotation), Is.LessThan(0.0001f));
		}
	}
}
