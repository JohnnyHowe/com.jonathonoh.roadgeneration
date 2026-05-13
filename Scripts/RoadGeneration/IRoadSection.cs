using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public interface IRoadSection
    {
        RoadSectionShape GetShape();
        void AlignByStartPoint(TransformData newStartPoint);
        IRoadSection Clone();
    }
}