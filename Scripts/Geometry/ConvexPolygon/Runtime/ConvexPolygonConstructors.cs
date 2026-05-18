using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.Geometry
{
	public static class ConvexHullConstructors
	{
		public static ConvexPolygon FromMesh(Mesh mesh, Vector3 projectionPlaneNormal)
		{
			return FromVertices3D(mesh.vertices, projectionPlaneNormal);
		}

		public static ConvexPolygon FromVertices3D(IEnumerable<Vector3> allVertices, Vector3 projectionPlaneNormal)
		{
			return new ConvexPolygon(ProjectAll(allVertices, projectionPlaneNormal));
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
