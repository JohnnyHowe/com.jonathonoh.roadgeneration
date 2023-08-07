using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Other.Tests
{
    public class ConvexHullUtilityTester
    {
        [Test]
        public void TestGetConvexHullTangentsUnitSquare()
        {
            List<Vector2> squarePoints = new List<Vector2>() {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
            List<Vector2> actualTangents = ConvexHullUtility2D.GetConvexHullTangents(squarePoints);
            List<Vector2> expected = new List<Vector2>() { new Vector2(0, 1), new Vector2(1, 0) };
            CollectionAssert.AreEquivalent(actualTangents, expected);
        }

        [Test]
        public void TestGetConvexHullTangentsUnitSquareOffset()
        {
            Vector2 offset = new Vector2(10, -15);
            List<Vector2> squarePoints = new List<Vector2>() {
            new Vector2(0, 0) + offset,
            new Vector2(0, 1) + offset,
            new Vector2(1, 1) + offset,
            new Vector2(1, 0) + offset
        };
            List<Vector2> actualTangents = ConvexHullUtility2D.GetConvexHullTangents(squarePoints);
            List<Vector2> expected = new List<Vector2>() { new Vector2(0, 1), new Vector2(1, 0) };
            CollectionAssert.AreEquivalent(actualTangents, expected);
        }

        [Test]
        public void TestGetConvexHullTangentsUnitSquareRotation45Degrees()
        {
            List<Vector2> squarePoints = new List<Vector2>() {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0),
        };
            List<Vector2> actualTangents = ConvexHullUtility2D.GetConvexHullTangents(squarePoints);
            List<Vector2> expected = new List<Vector2>() { new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized };
            CollectionAssert.AreEquivalent(actualTangents, expected);
        }

        [Test]
        public void TestGetConvexHullTangentsUnitSquareRotation45DegreesAndOffset()
        {
            Vector2 offset = new Vector2(10, -15);
            List<Vector2> squarePoints = new List<Vector2>() {
            new Vector2(0, 1) + offset,
            new Vector2(1, 0) + offset,
            new Vector2(0, -1) + offset,
            new Vector2(-1, 0) + offset
        };
            List<Vector2> actualTangents = ConvexHullUtility2D.GetConvexHullTangents(squarePoints);
            List<Vector2> expected = new List<Vector2>() { new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized };
            CollectionAssert.AreEquivalent(actualTangents, expected);
        }

        [Test]
        public void TestGetConvexHullTangentsUnitSquareWithRedundantPoints()
        {
            List<Vector2> squarePoints = new List<Vector2>() {
            new Vector2(0, 0),
            new Vector2(.5f, .5f),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(.4f, .7f),
            new Vector2(.3f, .7f),
            new Vector2(1, 0)
        };
            List<Vector2> actualTangents = ConvexHullUtility2D.GetConvexHullTangents(squarePoints);
            List<Vector2> expected = new List<Vector2>() { new Vector2(0, 1), new Vector2(1, 0) };
            CollectionAssert.AreEquivalent(actualTangents, expected);
        }
    }
}