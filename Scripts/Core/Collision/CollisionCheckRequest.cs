
using System.Collections.Generic;
using System.Linq;

namespace JonathonOH.RoadGeneration.Collision
{
	public struct CollisionCheckRequest
	{
		public readonly RoadSection Subject { get; init; }
		public readonly IReadOnlyList<RoadSection> AlreadyPlaced { get; init; }
		public readonly IReadOnlyList<RoadSection> Candidates { get; init; }

		public IReadOnlyList<RoadSection> ChainToCheckAgainst
		{
			get => AlreadyPlaced.Concat(Candidates).ToList();
		}

		public override string ToString()
		{
			string contents = string.Join(", ", new string[]
			{
				$"AlreadyPlaced={{ {GetRoadSectionsDisplayString(AlreadyPlaced)} }}",
				$"Candidates={{ {GetRoadSectionsDisplayString(Candidates)} }}",
				$"Subject={Subject.gameObject.name}",
			});
			return $"CollisionCheckRequest<{contents}>";
		}

		private static string GetRoadSectionsDisplayString(IEnumerable<RoadSection> roadSections)
		{
			return string.Join(", ", roadSections.Select(section => section.gameObject.name));
		}

		public IEnumerable<RoadSection> GetFullChain()
		{
			foreach (RoadSection section in AlreadyPlaced) yield return section;
			foreach (RoadSection section in Candidates) yield return section;
			yield return Subject;
		}
	}
}
