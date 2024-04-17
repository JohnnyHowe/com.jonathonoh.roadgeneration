using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSection : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private MeshFilter _boundingMesh;
        [SerializeField] private bool _infiniteHeight = false;
        [SerializeField] private List<Vector2> topology;

        [SerializeField][ReadOnly] public int PieceTypeId;
        [SerializeField][ReadOnly] public bool IsFlipped;

        private RoadSectionShape _shapeRelativeToStart
        {
            get
            {
                if (_localShapeReal == null) _SetShape();
                return _localShapeReal;
            }
            set
            {
                _localShapeReal = value;
            }
        }
        private RoadSectionShape _localShapeReal;

        private void OnDrawGizmos()
        {
            if (_localShapeReal != null) _shapeRelativeToStart.DebugDraw();
            _DrawEndPoints();
        }

        private void _SetShape()
        {
            _localShapeReal = new RoadSectionShape();
            _localShapeReal.Start = TransformData.FromTransform(_startPoint);
            _localShapeReal.End = TransformData.FromTransform(_endPoint);
            _localShapeReal.Start.Scale = Vector3.one;
            _localShapeReal.End.Scale = Vector3.one;
            _localShapeReal.SetBoundaryFromMesh(_boundingMesh.sharedMesh, TransformData.FromTransform(_boundingMesh.transform), _shapeRelativeToStart.Start, _infiniteHeight);
            _localShapeReal.DebugDraw();
        }

        void Update()
        {
            topology = _localShapeReal?._topologyGlobal.GetVertices();
        }

        private void _DrawEndPoints()
        {
            if (_startPoint != null)
            {
                Vector3 startDir = _startPoint.rotation * Quaternion.Euler(0, 0, 1).eulerAngles;
                DrawArrow.ForGizmo(_startPoint.position - startDir, startDir);
            }
            if (_endPoint != null)
            {
                DrawArrow.ForGizmo(_endPoint.position, _endPoint.rotation * Quaternion.Euler(0, 0, 1).eulerAngles);
            }
        }

        public void AlignByStartPoint(TransformData newStartPoint)
        {
            TransformData currentStart = TransformData.FromTransform(_startPoint);
            Vector3 rotationChange = newStartPoint.Rotation.eulerAngles - currentStart.Rotation.eulerAngles;
            transform.RotateAround(currentStart.Position, Vector3.up, rotationChange.y);
            Vector3 positionChange = newStartPoint.Position - currentStart.Position;
            transform.position += positionChange;
            _SetShape();
        }

        public void AlignByEndPoint(TransformData newEndPoint)
        {
            TransformData currentEnd = TransformData.FromTransform(_endPoint);
            Vector3 rotationChange = newEndPoint.Rotation.eulerAngles - currentEnd.Rotation.eulerAngles;
            transform.RotateAround(currentEnd.Position, Vector3.up, rotationChange.y);
            Vector3 positionChange = newEndPoint.Position - currentEnd.Position;
            transform.position += positionChange;
            _SetShape();
        }

        public RoadSection Clone()
        {
            GameObject clone = Instantiate(gameObject);
            clone.SetActive(true);
            return clone.GetComponent<RoadSection>();
        }

        public RoadSectionShape GetShape()
        {
            return _shapeRelativeToStart;
        }

        public void ClearShape()
        {
            _shapeRelativeToStart = null;
        }
    }
}