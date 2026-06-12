using JonathonOH.RoadGeneration.Core;
using System;

namespace JonathonOH.RoadGeneration
{
	[Serializable]
	public class RoadGeneratorChoiceEngine
	{
		private const int MAX_ITERATIONS = int.MaxValue;

		public ChoiceRequest CurrentChoiceRequest { get; private set; }
		public ChoiceResult? CurrentChoiceResult { get; private set; } = null;

		private ICollisionEngine collisionEngine;

		private int currentCandidateIndex = 0;
		private IRoadSection currentCandidate
		{
			get => CurrentChoiceRequest.SectionsInPreferenceOrder[currentCandidateIndex];
		}

		public RoadGeneratorChoiceEngine(ICollisionEngine collisionEngine)
		{
			this.collisionEngine = collisionEngine;
		}

		public void Reset(ChoiceRequest choiceRequest)
		{
			CurrentChoiceRequest = choiceRequest;
			CurrentChoiceResult = null;
			currentCandidateIndex = 0;
			ResetCollisionEngine();
		}

		public void StepUntilChoiceFound()
		{
			for (int i = 0; i < MAX_ITERATIONS; i++)
			{
				if (IsSearchFinished())
				{
					return;
				}
				Step();
			}
		}

		public void Step()
		{
			if (!IsSearching())
			{
				return;
			}

			collisionEngine.Step();
			ProcessCollisionEngineResult(collisionEngine.GetResult());
		}

		private void ProcessCollisionEngineResult(ICollisionEngine.SearchResult result)
		{
			if (result == ICollisionEngine.SearchResult.Impossible)
			{
				NoSolutionFoundForCurrentCandidate();
			}
			else if (result == ICollisionEngine.SearchResult.SolutionFound)
			{
				SolutionFoundForCurrentCandidate();
			}
		}

		private void NoSolutionFoundForCurrentCandidate()
		{
			if (currentCandidateIndex < CurrentChoiceRequest.SectionsInPreferenceOrder.Count - 1)
			{
				GoToNextCandidate();
			}
			else
			{
				// No more! Impossible search!
				CurrentChoiceResult = new ChoiceResult()
				{
					IsChoiceFound = false,
					ChosenSection = null,
					FailureReason = ChoiceResult.ChoiceFailureReason.NoChoiceFound
				};
			}
		}

		/// <summary>
		/// Assumes there is another candidate.
		/// </summary>
		private void GoToNextCandidate()
		{
			currentCandidateIndex++;
			ResetCollisionEngine();
		}

		private void ResetCollisionEngine()
		{
			CollisionCheckRequest request = CreateCollisionCheckRequestForCurrentCandidate();
			collisionEngine.Reset(request);
		}

		private CollisionCheckRequest CreateCollisionCheckRequestForCurrentCandidate()
		{
			return new CollisionCheckRequest()
			{
				Subject = currentCandidate,
				AlreadyPlaced = CurrentChoiceRequest.CurrentSectionsInWorld,
				MaxCheckDepth = CurrentChoiceRequest.MaxCheckDepth,
				AllowedSections = CurrentChoiceRequest.SectionsInPreferenceOrder
			};
		}

		private void SolutionFoundForCurrentCandidate()
		{
			CurrentChoiceResult = new ChoiceResult()
			{
				IsChoiceFound = true,
				ChosenSection = currentCandidate,
				FailureReason = ChoiceResult.ChoiceFailureReason.NoFailure
			};
		}

		public bool IsSearchFinished() => !IsSearching();
		public bool IsSearching() => CurrentChoiceResult == null;
	}
}
