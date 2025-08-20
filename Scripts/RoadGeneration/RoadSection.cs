using System.Collections.Generic;
using Other;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSection : MonoBehaviour
    {
        [SerializeField][ReadOnly] public int N;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private MeshFilter _boundingMesh;
        [SerializeField] private bool _infiniteHeight = false;
        [SerializeField] public bool autoFlip = true;

        [SerializeField][ReadOnly] public bool IsFlipped;
        [SerializeField][ReadOnly] public string pieceTypeId;

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

        protected void OnDrawGizmos()
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
        }

        public virtual void OnPoolObjectCreated() { }

        private void _DrawEndPoints()
        {
            if (_startPoint != null) _DrawPoint(_startPoint);
            if (_endPoint != null) _DrawPoint(_endPoint);
        }

        protected void _DrawPoint(Transform point)
        {
            Vector3 dir = point.rotation * Quaternion.Euler(0, 0, 1).eulerAngles;
            DrawArrow.ForGizmo(point.position - dir, dir, Color.white);
            DrawArrow.ForGizmo(point.position, point.rotation * Quaternion.Euler(0, 1, 0).eulerAngles * 0.5f, Color.green);
            DrawArrow.ForGizmo(point.position, point.rotation * Quaternion.Euler(1, 0, 0).eulerAngles * 0.5f, Color.red);
            DrawArrow.ForGizmo(point.position, dir, Color.blue);
        }

        public void AlignByStartPoint(TransformData newStartPoint)
        {
            TransformData currentStart = TransformData.FromTransform(_startPoint);
            Vector3 rotationChange = newStartPoint.Rotation.eulerAngles - currentStart.Rotation.eulerAngles;
            transform.RotateAround(currentStart.Position, Vector3.up, rotationChange.y);
            Vector3 positionChange = newStartPoint.Position - currentStart.Position;
            transform.position += positionChange;
            ResetShape();
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

        public void ResetShape()
        {
            _shapeRelativeToStart = null;
        }

        public void SetFlipped(bool flipped)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (flipped ? -1 : 1);
            transform.localScale = localScale;
            IsFlipped = flipped;
        }

        public string GetFullId()
        {
            return pieceTypeId + (IsFlipped ? "Flipped" : "");
        }
    }
}