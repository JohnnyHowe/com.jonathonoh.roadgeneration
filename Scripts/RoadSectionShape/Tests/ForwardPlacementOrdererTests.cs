using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration.Core;
using NUnit.Framework;
using UnityEngine;

namespace JonathonOH.RoadGeneration.RoadSectionShapeCollision.Tests
{
	public class ForwardPlacementOrdererTests
	{
		[Test]
		public void GetOrdered_WithNoSections_ReturnsNoSections()
		{
			IReadOnlyList<IRoadSection> ordered = GetOrdered();

			Assert.IsEmpty(ordered);
		}

		[Test]
		public void GetOrdered_WithOneSection_ReturnsThatSection()
		{
			TestRoadSection section = Section("straight", 1f, 1f);

			IReadOnlyList<IRoadSection> ordered = GetOrdered(section);

			CollectionAssert.AreEqual(new[] { section }, ordered);
		}

		[Test]
		public void GetOrdered_OrdersByStraightLeftRightCadence()
		{
			TestRoadSection straight = Section("straight", 1f, 1f);
			TestRoadSection softLeft = Section("soft-left", 1f, 1.5f);
			TestRoadSection hardLeft = Section("hard-left", 0f, 1f);
			TestRoadSection softRight = Section("soft-right", 3f, 1f);
			TestRoadSection hardRight = Section("hard-right", 1f, 0f);

			IReadOnlyList<IRoadSection> ordered = GetOrdered(straight, softLeft, hardLeft, softRight, hardRight);

			CollectionAssert.AreEqual(
				new[] { straight, hardLeft, hardRight, softLeft, softRight },
				ordered
			);
		}

		[Test]
		public void GetOrdered_WithOnlyLeftTurns_ReturnsStraightestThenHardestLeft()
		{
			TestRoadSection softLeft = Section("soft-left", 1f, 2f);
			TestRoadSection mediumLeft = Section("medium-left", 1f, 3f);
			TestRoadSection hardLeft = Section("hard-left", 0f, 1f);

			IReadOnlyList<IRoadSection> ordered = GetOrdered(softLeft, mediumLeft, hardLeft);

			CollectionAssert.AreEqual(
				new[] { softLeft, hardLeft, mediumLeft },
				ordered
			);
		}

		[Test]
		public void GetOrdered_WithOnlyRightTurns_ReturnsStraightestThenHardestRight()
		{
			TestRoadSection softRight = Section("soft-right", 2f, 1f);
			TestRoadSection mediumRight = Section("medium-right", 3f, 1f);
			TestRoadSection hardRight = Section("hard-right", 1f, 0f);

			IReadOnlyList<IRoadSection> ordered = GetOrdered(softRight, mediumRight, hardRight);

			CollectionAssert.AreEqual(
				new[] { softRight, hardRight, mediumRight },
				ordered
			);
		}

		[Test]
		public void GetOrdered_WithRepeatedSectionReference_ReturnsEachInputPosition()
		{
			TestRoadSection section = Section("same", 1f, 1f);

			IReadOnlyList<IRoadSection> ordered = GetOrdered(section, section);

			Assert.AreEqual(2, ordered.Count);
			Assert.AreSame(section, ordered[0]);
			Assert.AreSame(section, ordered[1]);
		}

		private static TestRoadSection Section(string id, float exitEulerX, float exitEulerZ)
		{
			return new TestRoadSection(id, exitEulerX, exitEulerZ);
		}

		private static IReadOnlyList<IRoadSection> GetOrdered(params TestRoadSection[] sections)
		{
			ForwardPlacementOrderer orderer = new ForwardPlacementOrderer(new ShapeCache());
			return orderer.GetOrdered(sections).ToList();
		}

		private class TestRoadSection : IRoadSection
		{
			public Pose Entry { get; }
			public Pose Exit { get; }
			public bool IsBoundaryInfiniteHeight => false;

			private readonly string id;

			public TestRoadSection(string id, float exitEulerX, float exitEulerZ)
			{
				this.id = id;
				Entry = new Pose(Vector3.zero, Quaternion.Euler(1f, 0f, 1f));
				Exit = new Pose(Vector3.forward, Quaternion.Euler(exitEulerX, 0f, exitEulerZ));
			}

			public IEnumerable<Vector3> GetBoundaryPoints()
			{
				yield return new Vector3(-0.5f, 0f, 0f);
				yield return new Vector3(0.5f, 0f, 0f);
				yield return new Vector3(0.5f, 0f, 1f);
				yield return new Vector3(-0.5f, 0f, 1f);
			}

			public string GetShapeId()
			{
				return id;
			}

			public override string ToString()
			{
				return id;
			}
		}
	}
}
