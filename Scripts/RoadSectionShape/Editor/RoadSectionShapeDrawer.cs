using JonathonOH.Geometry;
using JonathonOH.RoadGeneration.Core;
using JonathonOH.Spatial;
using UnityEditor;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision.Editor
{
	public static class RoadSectionShapeGizmoDrawer
	{
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		static void Draw(IRoadSection roadSection, GizmoType gizmoType)
		{
			if (Application.isPlaying)
			{
				return;
			}
			RoadSectionShape shape = RoadSectionShape.FromRoadSection(roadSection);
			ConvexHullDebugExtension.GizmosDraw(shape.Hull.HorizontalHull, Color.red, 0);

			PoseDebugExtensions.GizmosDraw(shape.Entry);
			PoseDebugExtensions.GizmosDraw(shape.Exit);
		}
	}
}
