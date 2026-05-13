
namespace JonathonOH.RoadGeneration
{
	public interface IAlignable
	{
        public void AlignStart(TransformData newStartPoint);
		public void AlignEnd(TransformData newEndPoint);
	}
}
