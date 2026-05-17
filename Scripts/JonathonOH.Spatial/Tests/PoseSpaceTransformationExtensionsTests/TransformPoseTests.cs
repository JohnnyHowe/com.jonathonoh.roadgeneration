using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Spatial.Tests.TransformPoseTests
{
	public static class TransformPoseTests
	{
		[Test]
		public static void IdentityParentPose_ReturnsLocalPoseUnchanged()
		{
			Pose parentPose = Pose.identity;
			Pose localPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			Pose expected = localPose;
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void IdentityLocalPose_ReturnsParentPoseUnchanged()
		{
			Pose parentPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			Pose localPose = Pose.identity;
			Pose expected = parentPose;
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void ParentTranslationWithIdentityRotations_OffsetsLocalPositionByParentPosition()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.identity);
			Pose localPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.identity);
			Pose expected = new Pose(new Vector3(11f, 22f, 33f), Quaternion.identity);
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void ParentRotationWithZeroParentTranslation_RotatesLocalPositionIntoWorldSpace()
		{
			Pose parentPose = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
			Pose localPose = new Pose(Vector3.forward, Quaternion.identity);
			Pose expected = new Pose(Vector3.right, Quaternion.Euler(0f, 90f, 0f));
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void ParentTranslationAndRotation_AppliesBothToLocalPosition()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			Pose localPose = new Pose(Vector3.forward, Quaternion.identity);
			Pose expected = new Pose(new Vector3(11f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void LocalRotation_ComposesAfterParentRotation()
		{
			Quaternion parentRotation = Quaternion.Euler(0f, 90f, 0f);
			Quaternion localRotation = Quaternion.Euler(90f, 0f, 0f);
			Pose parentPose = new Pose(Vector3.zero, parentRotation);
			Pose localPose = new Pose(Vector3.zero, localRotation);
			Pose expected = new Pose(Vector3.zero, parentRotation * localRotation);
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void NonAxisAlignedRotations_ComposeCorrectly()
		{
			Quaternion parentRotation = Quaternion.Euler(13f, 27f, 41f);
			Quaternion localRotation = Quaternion.Euler(-7f, 19f, 31f);
			Pose parentPose = new Pose(new Vector3(2f, -3f, 5f), parentRotation);
			Pose localPose = new Pose(new Vector3(1.5f, -2.25f, 0.75f), localRotation);
			Pose expected = new Pose(parentPose.position + parentRotation * localPose.position, parentRotation * localRotation);
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void NegativeLocalPosition_TransformsCorrectly()
		{
			Quaternion parentRotation = Quaternion.Euler(0f, 90f, 0f);
			Pose parentPose = new Pose(new Vector3(10f, 0f, 20f), parentRotation);
			Pose localPose = new Pose(new Vector3(-1f, -2f, -3f), Quaternion.identity);
			Pose expected = new Pose(parentPose.position + parentRotation * localPose.position, parentRotation);
			RunTest(parentPose, localPose, expected);
		}

		[Test]
		public static void ResultMatchesUnityTransformConfiguredWithSameParentPoseAndLocalPose()
		{
			var parent = new GameObject("Parent").transform;
			var child = new GameObject("Child").transform;
			try
			{
				Pose parentPose = new Pose(new Vector3(2f, -3f, 5f), Quaternion.Euler(13f, 27f, 41f));
				Pose localPose = new Pose(new Vector3(1.5f, -2.25f, 0.75f), Quaternion.Euler(-7f, 19f, 31f));

				parent.SetPositionAndRotation(parentPose.position, parentPose.rotation);
				child.SetParent(parent, false);
				child.localPosition = localPose.position;
				child.localRotation = localPose.rotation;

				Pose expected = new Pose(child.position, child.rotation);
				RunTest(parentPose, localPose, expected);
			}
			finally
			{
				Object.DestroyImmediate(child.gameObject);
				Object.DestroyImmediate(parent.gameObject);
			}
		}

		private static void RunTest(Pose thisPose, Pose localPose, Pose expected)
		{
			var result = thisPose.TransformPose(localPose);
			Assert.That(Vector3.Distance(expected.position, result.position), Is.LessThan(0.0001f));
			Assert.That(Quaternion.Angle(expected.rotation, result.rotation), Is.LessThan(0.0001f));
		}
	}
}
