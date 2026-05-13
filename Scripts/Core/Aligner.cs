
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
	public static class Aligner
	{
		public static void AlignByStart(RoadSection toAlign, TransformData newStart)
		{
			TransformData currentStart = toAlign.StartPoint;
			Vector3 rotationChange = newStart.Rotation.eulerAngles - currentStart.Rotation.eulerAngles;
			toAlign.transform.RotateAround(currentStart.Position, Vector3.up, rotationChange.y);
			Vector3 positionChange = newStart.Position - currentStart.Position;
			toAlign.transform.position += positionChange;
			toAlign.ResetShape();
		}
		public static void AlignByEnd(RoadSection toAlign, TransformData newEnd)
		{
			TransformData currentEnd = toAlign.EndPoint;
			Vector3 rotationChange = newEnd.Rotation.eulerAngles - currentEnd.Rotation.eulerAngles;
			toAlign.transform.RotateAround(currentEnd.Position, Vector3.up, rotationChange.y);
			Vector3 positionChange = newEnd.Position - currentEnd.Position;
			toAlign.transform.position += positionChange;
			toAlign.ResetShape();
		}
	}
}
