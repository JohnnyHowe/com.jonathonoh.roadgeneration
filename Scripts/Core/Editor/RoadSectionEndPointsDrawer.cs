using UnityEditor;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
	public static class RoadSectionEndPointsDrawer
	{
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		static void Draw(RoadSection roadSection, GizmoType gizmoType)
		{
			roadSection.Entry.DebugDraw();
			roadSection.Exit.DebugDraw();
		}
	}
}
