using System;

namespace JonathonOH.Geometry
{
    public readonly struct FloatRange 
    {
        public readonly float Min;
        public readonly float Max;

        public FloatRange(float value1, float value2)
        {
            Min = Math.Min(value1, value2);
            Max = Math.Max(value1, value2);
        }

        /// <summary>
        /// Returns true when this range and the other range share any values, including when they only touch at a boundary.
        /// </summary>
        public bool OverlapsWith(FloatRange other)
        {
            return Min <= other.Max && Max >= other.Min;
        }
    }
}
