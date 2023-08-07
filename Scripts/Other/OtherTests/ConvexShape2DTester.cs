using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Other.Tests
{
    public class ConvexShape2DTester
    {
        [Test]
        public void TestGetAxesUnitSquareAtOrigin()
        {
            List<Vector2> unitSquareAtOrigin = new List<Vector2> {
            new Vector2(-.5f, -.5f),
            new Vector2(-.5f, .5f),
            new Vector2(.5f, .5f),
            new Vector2(.5f, -.5f),
        };
            ConvexShape2D shape = new ConvexShape2D(unitSquareAtOrigin);

            List<Vector2> axes = shape.GetAxes();
            List<Vector2> expected = new List<Vector2>() { Vector2.right, Vector2.up };

            CollectionAssert.AreEquivalent(expected, axes);
        }

        [Test]
        public void DoesOverlapWithBothUnitSquaresAtOrigin()
        {
            List<Vector2> unitSquareAtOrigin1 = new List<Vector2> {
                new Vector2(-.5f, -.5f),
                new Vector2(-.5f, .5f),
                new Vector2(.5f, .5f),
                new Vector2(.5f, -.5f),
            };
            ConvexShape2D shape1 = new ConvexShape2D(unitSquareAtOrigin1);
            ConvexShape2D shape2 = new ConvexShape2D(unitSquareAtOrigin1);
            Assert.True(shape1.DoesOverlapWith(shape2));
        }

        [Test]
        public void DoesOverlapWithUnitSquaresPositionOffset()
        {
            List<Vector2> unitSquareAtOrigin = new List<Vector2> {
                new Vector2(-.5f, -.5f),
                new Vector2(-.5f, .5f),
                new Vector2(.5f, .5f),
                new Vector2(.5f, -.5f),
            };
            Vector2 offset = new Vector2(2, 2);
            List<Vector2> unitSquareOffset = new List<Vector2> {
                offset + new Vector2(-.5f, -.5f),
                offset + new Vector2(-.5f, .5f),
                offset + new Vector2(.5f, .5f),
                offset + new Vector2(.5f, -.5f),
            };
            ConvexShape2D shape1 = new ConvexShape2D(unitSquareAtOrigin);
            ConvexShape2D shape2 = new ConvexShape2D(unitSquareOffset);
            Assert.False(shape1.DoesOverlapWith(shape2));
        }
    }
}