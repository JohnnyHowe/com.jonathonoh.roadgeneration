using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public static class ConvexHullConstructors
	{
		public static ConvexHull FromMesh(Mesh mesh, Vector3 projectionPlaneNormal)
		{
			return FromVertices3D(mesh.vertices, projectionPlaneNormal);
		}

		public static ConvexHull FromVertices3D(IEnumerable<Vector3> allVertices, Vector3 projectionPlaneNormal)
		{
			return new ConvexHull(ProjectAll(allVertices, projectionPlaneNormal));
		}

		private static IEnumerable<Vector2> ProjectAll(IEnumerable<Vector3> points, Vector3 projectionPlaneNormal)
		{
			foreach (Vector3 point in points)
			{
				yield return Vector3.ProjectOnPlane(point, projectionPlaneNormal);
			}
		}
	}
}
