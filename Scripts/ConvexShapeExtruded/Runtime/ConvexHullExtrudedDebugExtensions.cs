using UnityEngine;

namespace JonathonOH.ConvexShapeExtruded
{
	public static class ConvexHullExtrudedDebugExtensions
	{
		private const float arrowHeadLength = 0.1f;
		private const float arrowLength = 0.4f;
		private const float arrowHeadAngle = 20;

		public static void DebugDraw(this ConvexHullExtruded target, Color color)
		{
			// Draw face at min vertical position if not -infinity
			if (target.VerticalRange.Min != -Mathf.Infinity)
			{
				target.HorizontalHull.DebugDraw(color, target.VerticalRange.Min);
			}

			// Draw face at max vertical position if not infinity
			if (target.VerticalRange.Max != Mathf.Infinity)
			{
				target.HorizontalHull.DebugDraw(color, target.VerticalRange.Max);
			}

			// Draw face at y=0 if both min and max vertical position +- infinity
			if (target.VerticalRange.Max == Mathf.Infinity && target.VerticalRange.Min == -Mathf.Infinity)
			{
				target.HorizontalHull.DebugDraw(color, 0);
			}

			DrawVerticalLines(target, color);
		}

		private static void DrawVerticalLines(ConvexHullExtruded target, Color color)
		{
			if (target.VerticalRange.Max != Mathf.Infinity && target.VerticalRange.Min != -Mathf.Infinity)
			{
				// Draw solid connecting lines
				DrawVerticalLines(target, target.VerticalRange.Min, target.VerticalRange.Max, color);
			}
			else if (target.VerticalRange.Min == -Mathf.Infinity)
			{
				DrawVerticalLines(target, target.VerticalRange.Max, -Mathf.Infinity, color);
			}
			else
			{
				DrawVerticalLines(target, target.VerticalRange.Min, Mathf.Infinity, color);
			}
		}

		/// <summary>
		/// if end is +-infinity, an arrow is drawn.
		/// </summary>
		private static void DrawVerticalLines(ConvexHullExtruded target, float start, float end, Color color)
		{
			foreach (Vector2 vertex in target.HorizontalHull.Vertices)
			{
				Vector3 point = new Vector3(vertex.x, start, vertex.y);

				if (end == Mathf.Infinity)
				{
					DrawArrow(point, Vector3.up, color);
				}
				else if (end == -Mathf.Infinity)
				{
					DrawArrow(point, Vector3.down, color);
				}
				else
				{
					Debug.DrawLine(point, new Vector3(vertex.x, end, vertex.y));
				}
			}
		}

		private static void DrawArrow(Vector3 start, Vector3 direction, Color color)
		{
			direction *= arrowLength;
			Vector3 end = start + direction;

			Debug.DrawRay(start, direction, color);

			Vector3 rightArrowHeadLine = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 leftArrowHeadLine = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Debug.DrawRay(end, rightArrowHeadLine * arrowHeadLength, color);
			Debug.DrawRay(end, leftArrowHeadLine * arrowHeadLength, color);
		}
	}
}
