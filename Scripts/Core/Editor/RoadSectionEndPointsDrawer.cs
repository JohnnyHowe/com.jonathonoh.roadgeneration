using UnityEditor;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
	public static class RoadSectionEndPointsDrawer
	{
		private const float arrowHeadLength = 1;
		private const float arrowLength = 4;
		private const float arrowHeadAngle = 20;

		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		static void Draw(RoadSection roadSection, GizmoType gizmoType)
		{
			DrawEndPoint(roadSection.StartPoint);
			DrawEndPoint(roadSection.EndPoint);
		}

		private static void DrawEndPoint(TransformData transformData)
		{
			DrawArrow(transformData.Position, transformData.Rotation * Vector3.forward, Color.blue);
			DrawArrow(transformData.Position, transformData.Rotation * Vector3.right, Color.red);
			DrawArrow(transformData.Position, transformData.Rotation * Vector3.up, Color.green);
		}

		private static void DrawArrow(Vector3 pos, Vector3 direction, Color color)
		{
			direction *= arrowLength;
			Debug.DrawRay(pos, direction, color);

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
			Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
		}
	}
}
