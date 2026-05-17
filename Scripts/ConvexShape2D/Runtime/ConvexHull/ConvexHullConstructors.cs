
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
	public static class ConvexHullConstructors
	{
		public static ConvexHull FromMesh(Mesh mesh, Vector3 projectionNormal)
		{
			return FromVertices3D(mesh.vertices, projectionNormal);
		}

		public static ConvexHull FromVertices3D(IEnumerable<Vector3> allVertices, Vector3 projectionNormal)
		{
			throw new NotImplementedException();
		}
	}
}
