using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Other.Tests
{
    public class DFSCombinationGeneratorTester
    {
        /// ===========================================================================================
        /// Other
        /// ===========================================================================================

        [Test]
        public void TestInitialState()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            CollectionAssert.AreEqual(new int[] { 0, -1, -1, -1, -1 }, generator.GetState());
        }

        [Test]
        public void TestInvalidOnFirst()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.StepInvalid();
        }

        /// ===========================================================================================
        /// GetCurrentDepth
        /// ===========================================================================================

        [Test]
        public void TestGetCurrentDepthEmpty()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { -1, -1, -1, -1, -1 });
            Assert.AreEqual(generator.GetCurrentDepth(), -1);
        }

        [Test]
        public void TestGetCurrentDepth()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 0, -1, -1, -1 });
            Assert.AreEqual(generator.GetCurrentDepth(), 1);
        }

        [Test]
        public void TestGetCurrentDepthAlmostMax()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 0, 0, 0, -1 });
            Assert.AreEqual(generator.GetCurrentDepth(), 3);
        }

        [Test]
        public void TestGetCurrentDepthMax()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 0, 0, 0, 0 });
            Assert.AreEqual(generator.GetCurrentDepth(), 4);
        }

        /// ===========================================================================================
        /// GetCurrentEnd
        /// ===========================================================================================

        [Test]
        public void TestGetCurrentEndPartialFull()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 1, 2, -1, -1 });
            Assert.AreEqual(2, generator.GetCurrentEnd());
        }

        [Test]
        public void TestGetCurrentEndEmpty()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { -1, -1, -1, -1, -1 });
            Assert.AreEqual(-1, generator.GetCurrentEnd());
        }

        [Test]
        public void TestGetCurrentEndFull()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 1, 2, 3, 4 });
            Assert.AreEqual(4, generator.GetCurrentEnd());
        }

        /// ===========================================================================================
        /// Has Found Solution
        /// ===========================================================================================

        [Test]
        public void TestHasFoundSolutionFalse()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 0, 0, -1, -1 });
            Assert.False(generator.HasFoundSolution());
        }

        [Test]
        public void TestHasFoundSolutionFalseFull()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 0, 0, 0, 0 });
            Assert.False(generator.HasFoundSolution());
        }

        [Test]
        public void TestHasFoundSolution()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 0, 0, 0, 0 });
            generator.StepValid();
            Assert.True(generator.HasFoundSolution());
        }

        /// ===========================================================================================
        /// StepValid
        /// ===========================================================================================

        [Test]
        public void TestStepValidOne()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.StepValid();
            CollectionAssert.AreEqual(new int[] { 0, 0, -1, -1, -1 }, generator.GetState());
        }

        [Test]
        public void TestStepValidThree()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.StepValid();
            generator.StepValid();
            generator.StepValid();
            CollectionAssert.AreEqual(new int[] { 0, 0, 0, 0, -1 }, generator.GetState());
        }

        [Test]
        public void TestStepInvalidOne()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(3, 3);
            generator.StepInvalid();
            CollectionAssert.AreEqual(new int[] { 1, -1, -1 }, generator.GetState());
        }

        /// ===========================================================================================
        /// StepInvalid
        /// ===========================================================================================

        [Test]
        public void TestStepInvalidThreeNoBackTrack()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.StepInvalid();
            generator.StepInvalid();
            generator.StepInvalid();
            CollectionAssert.AreEqual(new int[] { 3, -1, -1, -1, -1 }, generator.GetState());
        }

        [Test]
        public void TestStepInvalidSingleBackTrack()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 0, 4, -1, -1 });
            generator.StepInvalid();
            CollectionAssert.AreEqual(new int[] { 0, 1, -1, -1, -1 }, generator.GetState());
        }

        [Test]
        public void TestStepInvalidDoubleBackTrack()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 0, 4, 4, -1, -1 });
            generator.StepInvalid();
            CollectionAssert.AreEqual(new int[] { 1, -1, -1, -1, -1 }, generator.GetState());
        }

        [Test]
        public void TestStepInvalidBackTrackLast()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(2, 5);
            generator.SetState(new int[] { 0, 0, 0, 0, 1 });
            generator.StepInvalid();
            CollectionAssert.AreEqual(new int[] { 0, 0, 0, 1, -1 }, generator.GetState());
        }

        [Test]
        public void TestStepInvalidDoubleBackTrackToStart()
        {
            DFSCombinationGenerator generator = new DFSCombinationGenerator(5, 5);
            generator.SetState(new int[] { 4, 4, 4, -1, -1 });
            try
            {
                generator.StepInvalid();
                Assert.True(false, "An error should have been thrown");
            }
            catch (DFSCombinationGenerator.OutOfCombinationsException exception)
            {
                // Good, there was an error
            }
        }
    }
}