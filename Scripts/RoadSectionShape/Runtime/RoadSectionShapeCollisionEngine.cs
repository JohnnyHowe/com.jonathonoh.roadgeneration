using System;
using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Core;
using Other;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision
{
	/// <summary>
	/// Searches for a collision-free chain that starts with the already placed sections and requested subject section.
	/// </summary>
	public class RoadSectionShapeCollisionEngine : ICollisionEngine
	{
		private ShapeCache shapeCache;
		private RoadSectionShapeCollisionChecker collisionChecker;

		private CollisionCheckRequest? realRequest = null;
		private CollisionCheckRequest request => (CollisionCheckRequest)realRequest;

		private DFSCombinationGenerator combinationGenerator;

		private ICollisionEngine.SearchResult result = ICollisionEngine.SearchResult.SearchNotFinished;
		private bool hasCheckedSubject = false;

		public RoadSectionShapeCollisionEngine()
		{
			shapeCache = new ShapeCache();
			collisionChecker = new RoadSectionShapeCollisionChecker(shapeCache);
		}

		public void Reset(CollisionCheckRequest request)
		{
			realRequest = request;
			hasCheckedSubject = false;
			combinationGenerator = new DFSCombinationGenerator(request.AllowedSections.Count - 1, request.MaxCheckDepth);
			result = ICollisionEngine.SearchResult.SearchNotFinished;
		}

		private bool DoesSubjectCollideWithAlreadyPlacedSections()
		{
			return collisionChecker.GetCollidingSectionOrNull(request.AlreadyPlaced, request.Subject) != null;
		}

		public ICollisionEngine.SearchResult GetResult()
		{
			return result;
		}

		public void Step()
		{
			if (!hasCheckedSubject)
			{
				CheckSubject();
				hasCheckedSubject = true;
				return;
			}

			if (result != ICollisionEngine.SearchResult.SearchNotFinished)
			{
				return;
			}

			if (realRequest == null)
			{
				throw new ArgumentException("Cannot run collision engine. It hasn't been `Reset`!");
			}

			if (IsCurrentChainValid())
			{
				combinationGenerator.StepValid();
			}
			else
			{
				combinationGenerator.StepInvalid();
			}

			UpdateSearchResult();
		}

		private void CheckSubject()
		{
			if (DoesSubjectCollideWithAlreadyPlacedSections())
			{
				result = ICollisionEngine.SearchResult.Impossible;
			}
		}

		private bool IsCurrentChainValid()
		{
			var chain = GetCurrentChain().ToList();
			var collidingSection = collisionChecker.GetSectionLastCollidesWith(chain);
			return collidingSection == null;
		}

		private IEnumerable<IRoadSection> GetCurrentChain()
		{
			foreach (IRoadSection section in request.AlreadyPlaced)
			{
				yield return section;
			}

			yield return request.Subject;

			foreach (int allowedSectionIndex in combinationGenerator.GetState())
			{
				if (allowedSectionIndex != -1)
				{
					yield return request.AllowedSections[allowedSectionIndex];
				}
			}
		}

		private void UpdateSearchResult()
		{
			if (result != ICollisionEngine.SearchResult.SearchNotFinished)
			{
				return;
			}

			if (combinationGenerator == null)
			{
				result = ICollisionEngine.SearchResult.SearchNotFinished;
			}
			else if (combinationGenerator.HasFoundSolution())
			{
				result = ICollisionEngine.SearchResult.SolutionFound;
			}
			else if (combinationGenerator.IsImpossible())
			{
				result = ICollisionEngine.SearchResult.Impossible;
			}
		}
	}
}
