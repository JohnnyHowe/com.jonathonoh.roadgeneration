using UnityEditor;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
	public static class RoadSectionBoundaryDrawer
	{
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		static void Draw(RoadSection roadSection, GizmoType gizmoType)
		{
			Handles.color = Color.blue;
			DrawWireMesh(roadSection._boundingMesh);
			Handles.color = Color.red;
			if (roadSection._localShapeReal != null) roadSection._localShapeReal.DebugDraw();
		}

		static void DrawWireMesh(MeshFilter meshFilter)
		{
			var mesh = meshFilter.sharedMesh;
			var verts = mesh.vertices;
			var tris = mesh.triangles;
			var matrix = meshFilter.transform.localToWorldMatrix;

			for (int i = 0; i < tris.Length; i += 3)
			{
				Vector3 a = matrix.MultiplyPoint3x4(verts[tris[i]]);
				Vector3 b = matrix.MultiplyPoint3x4(verts[tris[i + 1]]);
				Vector3 c = matrix.MultiplyPoint3x4(verts[tris[i + 2]]);

				Handles.DrawLine(a, b);
				Handles.DrawLine(b, c);
				Handles.DrawLine(c, a);
			}
		}
	}
}
