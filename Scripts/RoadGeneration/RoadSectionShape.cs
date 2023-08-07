using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    /// <summary>
    /// Describes the shape of a road section
    /// Contains logic for bounding areas, and start and end position alignment.
    /// </summary>
    public class RoadSectionShape
    {
        public TransformData Start;
        public TransformData End;
        public List<Vector3> _boundaryVerticesRelativeToHandle;
        private FloatRange _heightRange;
        public ConvexShape2D _topologyGlobal;
        private bool _infiniteHeight;

        public void SetBoundaryFromMesh(Mesh mesh, TransformData meshGlobalTransform, TransformData handle, bool infiniteHeight = false)
        {
            _infiniteHeight = infiniteHeight;
            _boundaryVerticesRelativeToHandle = new List<Vector3>();
            Start = handle;
            foreach (Vector3 vertexLocalToMesh in mesh.vertices)
            {
                Vector3 vertexGlobal = meshGlobalTransform.TransformPoint(vertexLocalToMesh);
                Vector3 vertexLocalToHandle = Start.InverseTransformPoint(vertexGlobal);
                _boundaryVerticesRelativeToHandle.Add(vertexLocalToHandle);
            }
            RecalculateCollisionBoundaries();
        }

        public RoadSectionShape GetTranslatedCopy(TransformData newHandlePosition)
        {
            RoadSectionShape newShape = new RoadSectionShape();
            newShape.Start = newHandlePosition;
            newShape._boundaryVerticesRelativeToHandle = _boundaryVerticesRelativeToHandle;
            newShape.Start = newHandlePosition;
            newShape.End = newHandlePosition.TransformPoint(Start.InverseTransformPoint(End));
            newShape._infiniteHeight = _infiniteHeight;

            newShape.RecalculateCollisionBoundaries();
            return newShape;
        }

        public void RecalculateCollisionBoundaries()
        {
            List<Vector2> topology = new List<Vector2>();
            float _minHeight = Mathf.Infinity;
            float _maxHeight = -Mathf.Infinity;
            foreach (Vector3 vertex in _boundaryVerticesRelativeToHandle)
            {
                Vector3 globalVertex = Start.TransformPoint(vertex);
                _minHeight = Mathf.Min(globalVertex.y, _minHeight);
                _maxHeight = Mathf.Max(globalVertex.y, _maxHeight);
                topology.Add(new Vector2(globalVertex.x, globalVertex.z));
            }
            _topologyGlobal = new ConvexShape2D(topology);
            _heightRange = new FloatRange(_minHeight, _maxHeight);
        }

        public bool DoesOverlapWith(RoadSectionShape other)
        {
            if (!_infiniteHeight && !_heightRange.DoesOverlapWith(other._heightRange)) return false;
            if (!_topologyGlobal.DoesOverlapWith(other._topologyGlobal)) return false;
            return true;
        }

        public void DebugDraw()
        {
            List<Vector2> topology = _topologyGlobal.GetVertices();
            foreach (Vector2 vertex1 in topology)
            {
                Debug.DrawLine(new Vector3(vertex1.x, _heightRange.Min, vertex1.y), new Vector3(vertex1.x, _heightRange.Max, vertex1.y), Color.red);
                foreach (Vector2 vertex2 in topology)
                {
                    Debug.DrawLine(new Vector3(vertex1.x, _heightRange.Min, vertex1.y), new Vector3(vertex2.x, _heightRange.Min, vertex2.y), Color.red);
                    Debug.DrawLine(new Vector3(vertex1.x, _heightRange.Max, vertex1.y), new Vector3(vertex2.x, _heightRange.Max, vertex2.y), Color.red);
                }
            }
            foreach (Vector3 vertex1 in _boundaryVerticesRelativeToHandle)
            {
                foreach (Vector3 vertex2 in _boundaryVerticesRelativeToHandle)
                {
                    Debug.DrawLine(vertex1, vertex2);
                }
            }
        }
    }
}