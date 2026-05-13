using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Other;
using JonathonOH.RoadGeneration.ChoiceEngine;
using JonathonOH.RoadGeneration.Collision;

namespace JonathonOH.RoadGeneration
{
	/// <summary>
	/// Must call Reset() before anything else
	/// </summary>
	public class RoadGeneratorChoiceEngine
	{
		public DFSCombinationGenerator combinationGenerator;
		public ChoiceRequest CurrentChoiceRequest { get; private set; }
		public ChoiceResult CurrentChoiceResult { get; private set; }

		public bool IsSearching { get; private set; } = true;

		private const int MAX_ITERATIONS = 10000000;
		private ICollisionChecker collisionChecker;

		public RoadGeneratorChoiceEngine(ChoiceRequest choiceRequest, ICollisionChecker collisionChecker)
		{
			CurrentChoiceRequest = choiceRequest;
			combinationGenerator = new DFSCombinationGenerator(choiceRequest.SectionsInPreferenceOrder.Count, choiceRequest.MaxCheckDepth);
			this.collisionChecker = collisionChecker;

			CurrentChoiceResult = new ChoiceResult()
			{
				IsChoiceFound = false,
				ChosenSection = null,
				FailureReason = ChoiceResult.ChoiceFailureReason.SearchNotFinished
			};
		}

		public void StepUntilChoiceIsFound()
		{
			for (int i = 0; i < MAX_ITERATIONS; i++)
			{
				if (!IsSearching)
				{
					break;
				}
				Step();
			}
		}

		public void Step()
		{
			if (!IsSearching)
			{
				return;
			}

			RunCollisionCheckAndStepCombinationGenerator();
			CheckCombinationTermination();
		}

		private void CheckCombinationTermination()
		{
			if (combinationGenerator.HasFoundSolution())
			{
				CurrentChoiceResult = new ChoiceResult()
				{
					IsChoiceFound = true,
					ChosenSection = CurrentChoiceRequest.SectionsInPreferenceOrder[combinationGenerator.GetState()[0]],
					FailureReason = ChoiceResult.ChoiceFailureReason.NoFailure
				};
				IsSearching = false;
			}
			else if (combinationGenerator.IsImpossible())
			{
				CurrentChoiceResult = new ChoiceResult()
				{
					IsChoiceFound = false,
					ChosenSection = null,
					FailureReason = ChoiceResult.ChoiceFailureReason.NoChoiceFound
				};
				IsSearching = false;
			}
		}

		private void RunCollisionCheckAndStepCombinationGenerator()
		{
			CollisionCheckResult collisionCheckResult = GetCollisionResultForCurrentCandidates();
			if (collisionCheckResult.HasCollision)
			{
				Debug.Log(
					$"Invalid chain found. Candidate {collisionCheckResult.Request.Subject.gameObject.name} overlaps with {collisionCheckResult.CollidesWith.gameObject.name}\n" +
					$"Full chain: {string.Join(", ", collisionCheckResult.Request.GetFullChain().Select(section => section.gameObject.name))}"
				);
				combinationGenerator.StepInvalid();
			}
			else
			{
				combinationGenerator.StepValid();
			}
		}

		/// <summary>
		/// Null if no collision.
		/// </summary>
		private CollisionCheckResult GetCollisionResultForCurrentCandidates()
		{
			return collisionChecker.CheckOneAgainstMany(CreateCollisionCheckRequest());
		}

		private CollisionCheckRequest CreateCollisionCheckRequest()
		{
			List<RoadSection> allCandidates = GetCandidateRoadSections();

			IEnumerable<RoadSection> previousCandidates = allCandidates.Take(allCandidates.Count - 1);
			RoadSection currentCandidate = allCandidates.Last();

			return new CollisionCheckRequest()
			{
				Subject = currentCandidate,
				AlreadyPlaced = CurrentChoiceRequest.CurrentSectionsInWorld,
				Candidates = previousCandidates.ToList()
			};
		}

		private List<RoadSection> GetCandidateRoadSections()
		{
			List<RoadSection> candidates = new List<RoadSection>();
			foreach (int candidateChoiceIndex in combinationGenerator.GetState())
			{
				if (candidateChoiceIndex == -1) break;
				candidates.Add(CurrentChoiceRequest.SectionsInPreferenceOrder[candidateChoiceIndex]);
			}
			return candidates;
		}
	}
}
