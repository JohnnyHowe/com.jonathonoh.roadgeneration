using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Spatial.Tests.PoseExtensionTests
{
	public static class InverseTransformPoseTests
	{
		[Test]
		public static void IdentityParentPose_ReturnsWorldPoseUnchanged()
		{
			Pose parentPose = Pose.identity;
			Pose worldPose = new Pose(new Vector3(1f, 2f, 3f), Quaternion.Euler(10f, 20f, 30f));
			Pose expected = worldPose;
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void IdentityWorldPose_ReturnsInverseParentPose()
		{
			Quaternion parentRotation = Quaternion.Euler(10f, 20f, 30f);
			Pose parentPose = new Pose(new Vector3(1f, 2f, 3f), parentRotation);
			Pose worldPose = Pose.identity;
			Quaternion inverseParentRotation = Quaternion.Inverse(parentRotation);
			Pose expected = new Pose(inverseParentRotation * -parentPose.position, inverseParentRotation);
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void ParentTranslationWithIdentityRotations_OffsetsWorldPositionByNegativeParentPosition()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.identity);
			Pose worldPose = new Pose(new Vector3(11f, 22f, 33f), Quaternion.identity);
			Pose expected = new Pose(new Vector3(1f, 2f, 3f), Quaternion.identity);
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void ParentRotationWithZeroParentTranslation_RotatesWorldPositionIntoLocalSpace()
		{
			Pose parentPose = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
			Pose worldPose = new Pose(Vector3.right, Quaternion.Euler(0f, 90f, 0f));
			Pose expected = new Pose(Vector3.forward, Quaternion.identity);
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void ParentTranslationAndRotation_AppliesBothToWorldPosition()
		{
			Pose parentPose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			Pose worldPose = new Pose(new Vector3(11f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			Pose expected = new Pose(Vector3.forward, Quaternion.identity);
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void WorldRotation_ComposesAfterInverseParentRotation()
		{
			Quaternion parentRotation = Quaternion.Euler(0f, 90f, 0f);
			Quaternion localRotation = Quaternion.Euler(90f, 0f, 0f);
			Pose parentPose = new Pose(Vector3.zero, parentRotation);
			Pose worldPose = new Pose(Vector3.zero, parentRotation * localRotation);
			Pose expected = new Pose(Vector3.zero, localRotation);
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void NonAxisAlignedRotations_ComposeCorrectly()
		{
			Quaternion parentRotation = Quaternion.Euler(13f, 27f, 41f);
			Quaternion localRotation = Quaternion.Euler(-7f, 19f, 31f);
			Pose parentPose = new Pose(new Vector3(2f, -3f, 5f), parentRotation);
			Pose localPose = new Pose(new Vector3(1.5f, -2.25f, 0.75f), localRotation);
			Pose worldPose = new Pose(parentPose.position + parentRotation * localPose.position, parentRotation * localRotation);
			Pose expected = localPose;
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void NegativeWorldPosition_TransformsCorrectly()
		{
			Quaternion parentRotation = Quaternion.Euler(0f, 90f, 0f);
			Pose parentPose = new Pose(new Vector3(10f, 0f, 20f), parentRotation);
			Pose worldPose = new Pose(new Vector3(7f, -2f, 21f), parentRotation);
			Pose expected = new Pose(new Vector3(-1f, -2f, -3f), Quaternion.identity);
			RunTest(parentPose, worldPose, expected);
		}

		[Test]
		public static void ResultMatchesUnityTransformConfiguredWithSameParentPoseAndWorldPose()
		{
			var parent = new GameObject("Parent").transform;
			var child = new GameObject("Child").transform;
			try
			{
				Pose parentPose = new Pose(new Vector3(2f, -3f, 5f), Quaternion.Euler(13f, 27f, 41f));
				Pose worldPose = new Pose(new Vector3(1.5f, -2.25f, 0.75f), Quaternion.Euler(-7f, 19f, 31f));

				parent.SetPositionAndRotation(parentPose.position, parentPose.rotation);
				child.SetParent(parent, false);
				child.SetPositionAndRotation(worldPose.position, worldPose.rotation);

				Pose expected = new Pose(child.localPosition, child.localRotation);
				RunTest(parentPose, worldPose, expected);
			}
			finally
			{
				Object.DestroyImmediate(child.gameObject);
				Object.DestroyImmediate(parent.gameObject);
			}
		}

		private static void RunTest(Pose thisPose, Pose worldPose, Pose expected)
		{
			var result = thisPose.InverseTransformPose(worldPose);
			Assert.That(Vector3.Distance(expected.position, result.position), Is.LessThan(0.0001f));
			Assert.That(Quaternion.Angle(expected.rotation, result.rotation), Is.LessThan(0.0001f));
		}
	}
}
