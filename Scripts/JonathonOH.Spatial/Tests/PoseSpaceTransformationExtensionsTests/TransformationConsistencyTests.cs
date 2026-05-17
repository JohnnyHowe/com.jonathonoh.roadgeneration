using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Spatial.Tests.TransformationConsistencyTests
{
	public static class TransformationConsistencyTests
	{
		[Test]
		public static void IdentityParentPose_TransformPoseFollowedByInverseTransformPose_ReturnsOriginalLocalPose()
		{
			Pose parentPose = Pose.identity;
			Pose localPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			RunTransformThenInverseTest(parentPose, localPose);
		}

		[Test]
		public static void TranslatedParentPose_TransformPoseFollowedByInverseTransformPose_ReturnsOriginalLocalPose()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.identity);
			Pose localPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			RunTransformThenInverseTest(parentPose, localPose);
		}

		[Test]
		public static void RotatedParentPose_TransformPoseFollowedByInverseTransformPose_ReturnsOriginalLocalPose()
		{
			Pose parentPose = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
			Pose localPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			RunTransformThenInverseTest(parentPose, localPose);
		}

		[Test]
		public static void TranslatedAndRotatedParentPose_TransformPoseFollowedByInverseTransformPose_ReturnsOriginalLocalPose()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			Pose localPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			RunTransformThenInverseTest(parentPose, localPose);
		}

		[Test]
		public static void NonAxisAlignedPoses_TransformPoseFollowedByInverseTransformPose_ReturnsOriginalLocalPose()
		{
			Pose parentPose = new Pose(new Vector3(2f, -3f, 5f), Quaternion.Euler(13f, 27f, 41f));
			Pose localPose = new Pose(new Vector3(1.5f, -2.25f, 0.75f), Quaternion.Euler(-7f, 19f, 31f));
			RunTransformThenInverseTest(parentPose, localPose);
		}

		[Test]
		public static void NegativeLocalPosition_TransformPoseFollowedByInverseTransformPose_ReturnsOriginalLocalPose()
		{
			Pose parentPose = new Pose(new Vector3(10f, 0f, 20f), Quaternion.Euler(0f, 90f, 0f));
			Pose localPose = new Pose(new Vector3(-1f, -2f, -3f), Quaternion.Euler(10f, 20f, 30f));
			RunTransformThenInverseTest(parentPose, localPose);
		}

		[Test]
		public static void IdentityParentPose_InverseTransformPoseFollowedByTransformPose_ReturnsOriginalWorldPose()
		{
			Pose parentPose = Pose.identity;
			Pose worldPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			RunInverseThenTransformTest(parentPose, worldPose);
		}

		[Test]
		public static void TranslatedParentPose_InverseTransformPoseFollowedByTransformPose_ReturnsOriginalWorldPose()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.identity);
			Pose worldPose = new Pose(new Vector3(11f, 22f, 33f), Quaternion.Euler(10f, 20f, 30f));
			RunInverseThenTransformTest(parentPose, worldPose);
		}

		[Test]
		public static void RotatedParentPose_InverseTransformPoseFollowedByTransformPose_ReturnsOriginalWorldPose()
		{
			Pose parentPose = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
			Pose worldPose = new Pose(Vector3.right, Quaternion.Euler(10f, 110f, 30f));
			RunInverseThenTransformTest(parentPose, worldPose);
		}

		[Test]
		public static void TranslatedAndRotatedParentPose_InverseTransformPoseFollowedByTransformPose_ReturnsOriginalWorldPose()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			Pose worldPose = new Pose(new Vector3(11f, 20f, 30f), Quaternion.Euler(10f, 110f, 30f));
			RunInverseThenTransformTest(parentPose, worldPose);
		}

		[Test]
		public static void NonAxisAlignedPoses_InverseTransformPoseFollowedByTransformPose_ReturnsOriginalWorldPose()
		{
			Pose parentPose = new Pose(new Vector3(2f, -3f, 5f), Quaternion.Euler(13f, 27f, 41f));
			Pose worldPose = new Pose(new Vector3(1.5f, -2.25f, 0.75f), Quaternion.Euler(-7f, 19f, 31f));
			RunInverseThenTransformTest(parentPose, worldPose);
		}

		[Test]
		public static void NegativeWorldPosition_InverseTransformPoseFollowedByTransformPose_ReturnsOriginalWorldPose()
		{
			Pose parentPose = new Pose(new Vector3(10f, 0f, 20f), Quaternion.Euler(0f, 90f, 0f));
			Pose worldPose = new Pose(new Vector3(7f, -2f, 21f), Quaternion.Euler(10f, 20f, 30f));
			RunInverseThenTransformTest(parentPose, worldPose);
		}

		private static void RunTransformThenInverseTest(Pose parentPose, Pose localPose)
		{
			Pose worldPose = parentPose.TransformPose(localPose);
			Pose result = parentPose.InverseTransformPose(worldPose);

			AssertPoseEqual(localPose, result);
		}

		private static void RunInverseThenTransformTest(Pose parentPose, Pose worldPose)
		{
			Pose localPose = parentPose.InverseTransformPose(worldPose);
			Pose result = parentPose.TransformPose(localPose);

			AssertPoseEqual(worldPose, result);
		}

		private static void AssertPoseEqual(Pose expected, Pose result)
		{
			Assert.That(Vector3.Distance(expected.position, result.position), Is.LessThan(0.0001f));
			Assert.That(Quaternion.Angle(expected.rotation, result.rotation), Is.LessThan(0.0001f));
		}
	}
}
