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
			roadSection.StartPoint.DebugDraw();
			roadSection.EndPoint.DebugDraw();
		}
	}
}
