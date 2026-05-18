using UnityEngine;

namespace JonathonOH.Geometry
{
	public static class ConvexHullDebugExtension
	{
		public static void DebugDraw(this ConvexPolygon target, Color color)
		{
			target.DebugDraw(color, 0);
		}

		public static void DebugDraw(this ConvexPolygon target, Color color, float y)
		{
			for (int i = 0; i < target.Vertices.Count; i++)
			{
				int i2 = (i + 1) % target.Vertices.Count;

				Vector3 p1 = new Vector3(target.Vertices[i].x, y, target.Vertices[i].y);
				Vector3 p2 = new Vector3(target.Vertices[i2].x, y, target.Vertices[i2].y);

				Debug.DrawLine(p1, p2, color);
			}
		}

		public static void GizmosDraw(ConvexPolygon target, Color color, float y)
		{
			Gizmos.color = color;
			for (int i = 0; i < target.Vertices.Count; i++)
			{
				int i2 = (i + 1) % target.Vertices.Count;

				Vector3 p1 = new Vector3(target.Vertices[i].x, y, target.Vertices[i].y);
				Vector3 p2 = new Vector3(target.Vertices[i2].x, y, target.Vertices[i2].y);

				Gizmos.DrawLine(p1, p2);
			}
		}
	}
}
