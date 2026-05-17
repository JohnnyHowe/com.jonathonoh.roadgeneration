using UnityEngine;

namespace JonathonOH.Spatial
{
	public static class PoseDebugExtensions
	{
		private const float arrowHeadLength = 0.1f;
		private const float arrowLength = 0.4f;
		private const float arrowHeadAngle = 20;

		public static void DebugDraw(this Pose pose)
		{
			DrawArrow(pose.position, pose.forward, Color.blue);
			DrawArrow(pose.position, pose.right, Color.red);
			DrawArrow(pose.position, pose.up, Color.green);
		}

		private static void DrawArrow(Vector3 start, Vector3 direction, Color color)
		{
			Vector3 end = start + direction;
			direction *= arrowLength;

			Debug.DrawRay(start, direction, color);

			Vector3 rightArrowHeadLine = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 leftArrowHeadLine = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Debug.DrawRay(end, rightArrowHeadLine * arrowHeadLength, color);
			Debug.DrawRay(end, leftArrowHeadLine * arrowHeadLength, color);
		}
	}
}
