using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration.ConvexShape2D
{
    public class Shape
    {
        private List<Vector2> _vertices;
        private List<Vector2> _axes;

        public Shape(List<Vector2> vertices)
        {
            _vertices = vertices;
            _axes = ConvexHullUtility2D.GetConvexHullAxes(vertices);
        }

        public List<Vector2> GetAxes()
        {
            return _axes;
        }

        public FloatRange GetProjection(Vector2 axis)
        {
            float min = Mathf.Infinity;
            float max = -Mathf.Infinity;
            foreach (Vector2 vertex in _vertices)
            {
                float vertexProjection = _Project(axis, vertex);
                min = Mathf.Min(min, vertexProjection);
                max = Mathf.Max(max, vertexProjection);
            }
            return new FloatRange(min, max);
        }

        public List<Vector2> GetVertices()
        {
            return _vertices;
        }

        private static float _Project(Vector2 axis, Vector2 point)
        {
            return Vector2.Dot(axis, point) / axis.magnitude;
        }

        public bool DoesOverlapWith(Shape other)
        {
            // for each axis of both objects
            IEnumerable<Vector2> axes = GetAxes().Concat(other.GetAxes()).Distinct().ToList();
            foreach (Vector2 axis in axes)
            {
                // get the projection of each object on the axis
                FloatRange thisProjectionRange = GetProjection(axis);
                FloatRange otherProjectionRange = other.GetProjection(axis);
                // if there is a gap, return false - there is no overlap
                if (!thisProjectionRange.OverlapsWith(otherProjectionRange)) return false;
            }
            return true;
        }
    }
}
