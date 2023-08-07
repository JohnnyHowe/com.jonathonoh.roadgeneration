using System;

namespace Other
{
    public class FloatRange : IEquatable<FloatRange>
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public FloatRange(float value1, float value2)
        {
            Set(value1, value2);
        }

        public void Set(float value1, float value2)
        {
            Min = Math.Min(value1, value2);
            Max = Math.Max(value1, value2);
        }

        public bool DoesOverlapWith(FloatRange other)
        {
            return Min <= other.Max && Max >= other.Min;
        }

        public bool Equals(FloatRange other)
        {
            return other.Min == Min && other.Max == Max;
        }

        public override bool Equals(object obj) => this.Equals(obj as FloatRange);

        public override int GetHashCode() => (Min, Max).GetHashCode();

        public static bool operator ==(FloatRange lhs, FloatRange rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FloatRange lhs, FloatRange rhs) => !(lhs == rhs);
    }
}