using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Other
{
    /// <summary>
    /// Ugly ass code
    /// Definitely not AI generated
    /// Does it really matter? It's tested good
    /// </summary>
    public static class ConvexHullUtility2D
    {
        private class Vector2EqualityComparer : IEqualityComparer<Vector2>
        {
            public bool Equals(Vector2 v1, Vector2 v2)
            {
                return Vector2.Distance(v1, v2) < Mathf.Epsilon;
            }

            public int GetHashCode(Vector2 vector)
            {
                return vector.GetHashCode();
            }
        }

        public static List<Vector2> GetConvexHullAxes(List<Vector2> points)
        {
            List<Vector2> axes = new List<Vector2>();
            foreach (Vector2 tangent in GetConvexHullTangents(points))
            {
                Vector2 axis = new Vector2(-tangent.y, tangent.x).normalized;
                if (axis.magnitude == 0) continue;
                if (axis.x < 0) axis = -axis;
                if (axis.y < 0) axis = -axis;
                axes.Add(axis);
            }
            return axes.Distinct().ToList();
        }

        public static List<Vector2> GetConvexHullTangents(List<Vector2> points)
        {
            List<Vector2> tangents = new List<Vector2>();
            foreach ((Vector2, Vector2) edge in GetConvexHull(points))
            {
                Vector2 tangent = edge.Item1 - edge.Item2;

                // Ensure up (and if horizontal, right)
                if (tangent.x < 0) tangent = -tangent;
                if (tangent.y < 0) tangent = -tangent;

                tangents.Add(tangent.normalized);
            }
            return tangents.Distinct(new Vector2EqualityComparer()).ToList(); ;
        }

        public static List<(Vector2, Vector2)> GetConvexHull(List<Vector2> points)
        {
            if (points.Count < 3)
                return new List<(Vector2, Vector2)>();

            Vector2 leftmost = FindLeftmostPoint(points);
            List<Vector2> sortedPoints = SortByPolarAngle(points, leftmost);
            Stack<Vector2> hullStack = new Stack<Vector2>();
            hullStack.Push(sortedPoints[0]);
            hullStack.Push(sortedPoints[1]);

            for (int i = 2; i < sortedPoints.Count; i++)
            {
                Vector2 top = hullStack.Pop();
                while (hullStack.Count > 0 && !IsCounterClockwise(hullStack.Peek(), top, sortedPoints[i]))
                {
                    top = hullStack.Pop();
                }
                hullStack.Push(top);
                hullStack.Push(sortedPoints[i]);
            }

            List<(Vector2, Vector2)> hullEdges = new List<(Vector2, Vector2)>();
            Vector2 prevPoint = hullStack.Pop();
            while (hullStack.Count > 0)
            {
                Vector2 currentPoint = hullStack.Pop();
                hullEdges.Add((prevPoint, currentPoint));
                prevPoint = currentPoint;
            }

            return hullEdges;
        }

        private static Vector2 FindLeftmostPoint(List<Vector2> points)
        {
            Vector2 leftmost = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].x < leftmost.x || (Mathf.Approximately(points[i].x, leftmost.x) && points[i].y < leftmost.y))
                {
                    leftmost = points[i];
                }
            }
            return leftmost;
        }

        private static List<Vector2> SortByPolarAngle(List<Vector2> points, Vector2 pivot)
        {
            List<Vector2> sortedPoints = new List<Vector2>(points);
            sortedPoints.Sort((a, b) =>
            {
                float angleA = Mathf.Atan2(a.y - pivot.y, a.x - pivot.x);
                float angleB = Mathf.Atan2(b.y - pivot.y, b.x - pivot.x);
                return angleA.CompareTo(angleB);
            });
            return sortedPoints;
        }

        private static bool IsCounterClockwise(Vector2 a, Vector2 b, Vector2 c)
        {
            float crossProduct = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
            return crossProduct > 0;
        }

        // Helper function to get edges from the convex hull
        private static List<(Vector2, Vector2)> GetEdges(List<Vector2> hullPoints)
        {
            List<(Vector2, Vector2)> edges = new List<(Vector2, Vector2)>();
            int numPoints = hullPoints.Count;

            for (int i = 0; i < numPoints; i++)
            {
                Vector2 currentPoint = hullPoints[i];
                Vector2 nextPoint = hullPoints[(i + 1) % numPoints]; // Wrap around to the first point

                edges.Add((currentPoint, nextPoint));
            }

            return edges;
        }
    }
}