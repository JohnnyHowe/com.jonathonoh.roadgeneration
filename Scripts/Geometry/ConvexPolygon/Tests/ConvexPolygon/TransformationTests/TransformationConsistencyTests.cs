using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.Geometry.Tests.ConvexPolygonTests.TransformationTests
{
	internal class TransformationConsistencyTests
	{
		[Test]
		public void IdentityPose_TransformByFollowedByInverseTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = Pose.identity;
			RunTransformThenInverseTest(hull, pose);
		}

		[Test]
		public void TranslatedPose_TransformByFollowedByInverseTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.identity);
			RunTransformThenInverseTest(hull, pose);
		}

		[Test]
		public void RotatedPose_TransformByFollowedByInverseTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
			RunTransformThenInverseTest(hull, pose);
		}

		[Test]
		public void TranslatedAndRotatedPose_TransformByFollowedByInverseTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			RunTransformThenInverseTest(hull, pose);
		}

		[Test]
		public void NonAxisAlignedYawRotation_TransformByFollowedByInverseTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(1f, 1f));
			Pose pose = new Pose(new Vector3(3f, 0f, -4f), Quaternion.Euler(0f, 30f, 0f));
			RunTransformThenInverseTest(hull, pose);
		}

		[Test]
		public void NegativeVertices_TransformByFollowedByInverseTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(-2f, -1f),
				new Vector2(1f, -1f),
				new Vector2(0f, 2f));
			Pose pose = new Pose(new Vector3(10f, 0f, 20f), Quaternion.Euler(0f, 90f, 0f));
			RunTransformThenInverseTest(hull, pose);
		}

		[Test]
		public void IdentityPose_InverseTransformByFollowedByTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(2f, 0f),
				new Vector2(2f, 1f),
				new Vector2(0f, 1f));
			Pose pose = Pose.identity;
			RunInverseThenTransformTest(hull, pose);
		}

		[Test]
		public void TranslatedPose_InverseTransformByFollowedByTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(10f, 30f),
				new Vector2(12f, 30f),
				new Vector2(12f, 31f),
				new Vector2(10f, 31f));
			Pose pose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.identity);
			RunInverseThenTransformTest(hull, pose);
		}

		[Test]
		public void RotatedPose_InverseTransformByFollowedByTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(0f, 0f),
				new Vector2(0f, -2f),
				new Vector2(1f, -2f),
				new Vector2(1f, 0f));
			Pose pose = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
			RunInverseThenTransformTest(hull, pose);
		}

		[Test]
		public void TranslatedAndRotatedPose_InverseTransformByFollowedByTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(10f, 30f),
				new Vector2(10f, 28f),
				new Vector2(11f, 28f),
				new Vector2(11f, 30f));
			Pose pose = new Pose(new Vector3(10f, 20f, 30f), Quaternion.Euler(0f, 90f, 0f));
			RunInverseThenTransformTest(hull, pose);
		}

		[Test]
		public void NonAxisAlignedYawRotation_InverseTransformByFollowedByTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(3f, -4f),
				new Vector2(4.732051f, -5f),
				new Vector2(4.366025f, -3.633975f));
			Pose pose = new Pose(new Vector3(3f, 0f, -4f), Quaternion.Euler(0f, 30f, 0f));
			RunInverseThenTransformTest(hull, pose);
		}

		[Test]
		public void NegativeVertices_InverseTransformByFollowedByTransformBy_ReturnsOriginalHull()
		{
			ConvexPolygon hull = CreateHull(
				new Vector2(9f, 22f),
				new Vector2(9f, 19f),
				new Vector2(12f, 20f));
			Pose pose = new Pose(new Vector3(10f, 0f, 20f), Quaternion.Euler(0f, 90f, 0f));
			RunInverseThenTransformTest(hull, pose);
		}

		private static void RunTransformThenInverseTest(ConvexPolygon hull, Pose pose)
		{
			ConvexPolygon transformedHull = hull.TransformBy(pose);
			ConvexPolygon result = transformedHull.InverseTransformBy(pose);
			AssertHullVerticesEqual(hull, result);
		}

		private static void RunInverseThenTransformTest(ConvexPolygon hull, Pose pose)
		{
			ConvexPolygon transformedHull = hull.InverseTransformBy(pose);
			ConvexPolygon result = transformedHull.TransformBy(pose);
			AssertHullVerticesEqual(hull, result);
		}

		private static ConvexPolygon CreateHull(params Vector2[] points)
		{
			return new ConvexPolygon(points);
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
