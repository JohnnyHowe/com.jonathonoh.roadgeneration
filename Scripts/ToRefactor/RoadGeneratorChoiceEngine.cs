using JonathonOH.RoadGeneration.ChoiceEngine;
using JonathonOH.RoadGeneration.Collision;
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
		private RoadSection currentCandidate
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

			CollisionCheckResult? result = collisionEngine.GetResult();
			if (result != null)
			{
				ProcessCollisionCheckResult((CollisionCheckResult)result);
			}
		}

		private void ProcessCollisionCheckResult(CollisionCheckResult result)
		{
			if (result.HasCollision)
			{
				ProcessCollisionCheckResultWithCollision();
			}
			else
			{
				ProcessCollisionCheckResultWithoutCollision(result);
			}
		}

		private void ProcessCollisionCheckResultWithCollision()
		{
			if (currentCandidateIndex < CurrentChoiceRequest.SectionsInPreferenceOrder.Count)
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

			CollisionCheckRequestNew request = CreateCollisionCheckRequestForCurrentCandidate();
			collisionEngine.Reset(request);
		}

		private CollisionCheckRequestNew CreateCollisionCheckRequestForCurrentCandidate()
		{
			return new CollisionCheckRequestNew()
			{
				Subject = currentCandidate,
				AlreadyPlaced = CurrentChoiceRequest.CurrentSectionsInWorld,
				MaxCheckDepth = CurrentChoiceRequest.MaxCheckDepth
			};
		}

		private void ProcessCollisionCheckResultWithoutCollision(CollisionCheckResult result)
		{
			CurrentChoiceResult = new ChoiceResult()
			{
				IsChoiceFound = true,
				ChosenSection = result.Request.Subject,
				FailureReason = ChoiceResult.ChoiceFailureReason.NoFailure
			};
		}

		public bool IsSearchFinished() => !IsSearching();
		public bool IsSearching() => CurrentChoiceResult != null;
	}
}
