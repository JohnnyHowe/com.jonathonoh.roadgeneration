using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    [System.Serializable]
    public struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Transform point from local space to world space
        /// </summary>
        public Vector3 TransformPoint(Vector3 point)
        {
            point = new Vector3(point.x * Scale.x, point.y * Scale.y, point.z * Scale.z);
            point = Rotation * point;
            point += Position;
            return point;
        }

        public TransformData TransformPoint(TransformData point)
        {
            return new TransformData(
                TransformPoint(point.Position),
                Rotation * point.Rotation,
                Vector3.Scale(Scale, point.Scale)
            );
        }

        /// <summary>
        /// Transform point from world space to local space
        /// </summary>
        public Vector3 InverseTransformPoint(Vector3 point)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(Position, Rotation, Scale);
            Matrix4x4 inverse = matrix.inverse;
            return inverse.MultiplyPoint3x4(point);
        }

        public TransformData InverseTransformPoint(TransformData point)
        {
            // TODO make work with scale
            if (point.Scale != Vector3.one || Scale != Vector3.one)
            {
                Debug.LogWarning("TransformData.InverseTransformPoint does not work when scale is not one!");
            }

            Matrix4x4 matrix = Matrix4x4.TRS(Position, Rotation, Scale);
            Matrix4x4 inverse = matrix.inverse;
            
            return new TransformData(
                inverse.MultiplyPoint3x4(point.Position),
                Quaternion.Inverse(Rotation) * point.Rotation,
                new Vector3(point.Scale.x / Scale.x, point.Scale.y / Scale.y, point.Scale.z / Scale.z)
            );
        }

        public bool Equals(TransformData other)
        {
            return Position == other.Position && Rotation == other.Rotation && Scale == other.Scale;
        }

        public static TransformData FromTransform(Transform transform)
        {
            return new TransformData(transform.position, transform.rotation, transform.lossyScale);
        }
    }
}