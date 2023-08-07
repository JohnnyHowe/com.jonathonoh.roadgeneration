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
            Vector3 startDir = _startPoint.rotation * Quaternion.Euler(0, 0, 1).eulerAngles;
            DrawArrow.ForGizmo(_startPoint.position - startDir, startDir);
            DrawArrow.ForGizmo(_endPoint.position, _endPoint.rotation * Quaternion.Euler(0, 0, 1).eulerAngles);
        }

        public void AlignByStartPoint(TransformData newStartPoint)
        {
            transform.position += newStartPoint.Position;
            transform.rotation = newStartPoint.Rotation;
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
    }
}