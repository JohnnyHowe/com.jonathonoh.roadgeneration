using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.ConvexPolygonTests.TransformationTests
{
	internal class TransformByTests
	{
		[Test]
		public void IdentityPose_ReturnsHullUnchanged()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = Pose.identity;
			ConvexPolygon expected = hull;
			RunTest(hull, pose, expected);
		}

		[Test]
		public void TranslatedPose_OffsetsHullByPosePositionXZ()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.identity);
			ConvexPolygon expected = CreateHull(
				new Vector2(10f, 30f),
				new Vector2(12f, 30f),
				new Vector2(12f, 31f),
				new Vector2(10f, 31f));
			RunTest(hull, pose, expected);
		}

		[Test]
		public void RotatedPose_RotatesHullAroundYAxis()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
			ConvexPolygon expected = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(0f, -2f),
				new Vector2(1f, -2f),
				new Vector2(1f, 0f));
			RunTest(hull, pose, expected);
		}

		[Test]
		public void TranslatedAndRotatedPose_AppliesBothToHullVertices()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			ConvexPolygon expected = CreateHull(
				new Vector2(10f, 30f),
				new Vector2(10f, 28f),
				new Vector2(11f, 28f),
				new Vector2(11f, 30f));
			RunTest(hull, pose, expected);
		}

		[Test]
		public void NonAxisAlignedYawRotation_TransformsHullVerticesCorrectly()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(1f, 1f));
			Pose pose = new Pose(new Vector3(3f, 0f, -4f), Quaternion.Euler(0f, 30f, 0f));
			ConvexPolygon expected = CreateHull(
				TransformPoint(new Vector2(0f, 0f), pose),
				TransformPoint(new Vector2(2f, 0f), pose),
				TransformPoint(new Vector2(1f, 1f), pose));
			RunTest(hull, pose, expected);
		}

		[Test]
		public void NegativeVertices_TransformsCorrectly()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(-2f, -1f),
				new Vector2(1f, -1f),
				new Vector2(0f, 2f));
			Pose pose = new Pose(new Vector3(10f, 0f, 20f), Quaternion.Euler(0f, 90f, 0f));
			ConvexPolygon expected = CreateHull(
				TransformPoint(new Vector2(-2f, -1f), pose),
				TransformPoint(new Vector2(1f, -1f), pose),
				TransformPoint(new Vector2(0f, 2f), pose));
			RunTest(hull, pose, expected);
		}

		[Test]
		public void ResultMatchesUnityTransformConfiguredWithSamePose()
		{
			var transform = new GameObject("Hull Transform").transform;
			try
			{
				ConvexPolygon hull = CreateHull(
					new Vector2(0f, 0f),
					new Vector2(2f, 0f),
					new Vector2(1f, 1f));
				Pose pose = new Pose(new Vector3(3f, 4f, -5f), Quaternion.Euler(0f, 37f, 0f));

				transform.SetPositionAndRotation(pose.position, pose.rotation);
				ConvexPolygon expected = CreateHull(
					TransformPointWithUnityTransform(new Vector2(0f, 0f), transform),
					TransformPointWithUnityTransform(new Vector2(2f, 0f), transform),
					TransformPointWithUnityTransform(new Vector2(1f, 1f), transform));
				RunTest(hull, pose, expected);
			}
			finally
			{
				Object.DestroyImmediate(transform.gameObject);
			}
		}

		private static void RunTest(ConvexPolygon hull, Pose pose, ConvexPolygon expected)
		{
			ConvexPolygon result = hull.TransformBy(pose);
			AssertHullVerticesEqual(expected, result);
		}

		private static ConvexPolygon CreateHull(params Vector2[] points)
		{
			return new ConvexPolygon(points);
		}

		private static Vector2 TransformPoint(Vector2 point, Pose pose)
		{
			Vector3 transformedPoint = pose.position + pose.rotation * new Vector3(point.x, 0f, point.y);
			return new Vector2(transformedPoint.x, transformedPoint.z);
		}

		private static Vector2 TransformPointWithUnityTransform(Vector2 point, Transform transform)
		{
			Vector3 transformedPoint = transform.TransformPoint(new Vector3(point.x, 0f, point.y));
			return new Vector2(transformedPoint.x, transformedPoint.z);
		}

		private static void AssertHullVerticesEqual(ConvexPolygon expected, ConvexPolygon result)
		{
			Assert.That(result.Vertices.Count, Is.EqualTo(expected.Vertices.Count));

			List<Vector2> remainingVertices = new List<Vector2>(result.Vertices);
			foreach (Vector2 expectedVertex in expected.Vertices)
			{
				int matchingIndex = remainingVertices.FindIndex(vertex => Vector2.Distance(expectedVertex, vertex) < 0.0001f);
				Assert.That(matchingIndex, Is.Not.EqualTo(-1), $"Missing expected vertex {expectedVertex}");
				remainingVertices.RemoveAt(matchingIndex);
			}
		}
	}
}
