using System;
using JonathonOH.RoadGeneration.ConvexShape2D;
using UnityEngine;

namespace JonathonOH.ConvexShapeExtruded
{
	public readonly struct ConvexHullExtruded
	{
		public readonly ConvexHull HorizontalHull;
		public readonly FloatRange VerticalRange;

		public static ConvexHullExtruded FromMesh(Mesh mesh)
		{
			throw new NotImplementedException();
		}

		public ConvexHullExtruded TransformBy(Pose pose)
		{
			throw new NotImplementedException();
		}

		public ConvexHullExtruded InverseTransformBy(Pose pose)
		{
			throw new NotImplementedException();
		}

		public bool OverlapsWith(ConvexHullExtruded other)
		{
			throw new NotImplementedException();
		}

		public void DebugDraw(Color color)
		{

		}
	}
}
