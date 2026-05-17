using UnityEngine;

namespace JonathonOH.Spatial
{
	public static class PoseContructors
	{
		public static Pose FromTransform(Transform transform)
		{
			return new Pose(transform.position, transform.rotation);
		}
	}
}
