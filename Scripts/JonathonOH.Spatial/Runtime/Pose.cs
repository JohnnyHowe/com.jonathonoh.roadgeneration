using UnityEngine;

namespace JonathonOH.Spatial
{
	/// <summary>
	/// A Pose represents a position and rotation in space
	/// https://en.wikipedia.org/wiki/Pose_(computer_vision)
	/// </summary>
	[System.Serializable]
	public struct Pose
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public Pose(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}

		/// <summary>
		/// Transform point from local space to world space
		/// </summary>
		public Vector3 TransformPoint(Vector3 point)
		{
			point = Rotation * point;
			point += Position;
			return point;
		}

		public Pose TransformPoint(Pose point)
		{
			return new Pose(
				TransformPoint(point.Position),
				Rotation * point.Rotation
			);
		}

		/// <summary>
		/// Transform point from world space to local space
		/// </summary>
		public Vector3 InverseTransformPoint(Vector3 point)
		{
			Matrix4x4 matrix = Matrix4x4.TRS(Position, Rotation, Vector3.one);
			Matrix4x4 inverse = matrix.inverse;
			return inverse.MultiplyPoint3x4(point);
		}

		public Pose InverseTransformPoint(Pose point)
		{
			Matrix4x4 matrix = Matrix4x4.TRS(Position, Rotation, Vector3.one);
			Matrix4x4 inverse = matrix.inverse;

			return new Pose(
				inverse.MultiplyPoint3x4(point.Position),
				Quaternion.Inverse(Rotation) * point.Rotation
			);
		}

		public static Pose FromTransform(Transform transform)
		{
			return new Pose(transform.position, transform.rotation);
		}

		public static Pose Default()
		{
			return new Pose(Vector3.zero, Quaternion.identity);
		}

		public void DebugDraw()
		{
			DrawArrow(Position, Rotation * Vector3.forward, Color.blue);
			DrawArrow(Position, Rotation * Vector3.right, Color.red);
			DrawArrow(Position, Rotation * Vector3.up, Color.green);
		}

		private static void DrawArrow(Vector3 pos, Vector3 direction, Color color)
		{
			float arrowHeadLength = 0.1f;
			float arrowLength = 0.4f;
			float arrowHeadAngle = 20;

			direction *= arrowLength;
			Debug.DrawRay(pos, direction, color);

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
			Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
		}
	}
}
