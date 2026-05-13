using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Other.Tests
{
    public class FloatRangeTester
    {
        [Test]
        public void TestConstructorBasic()
        {
            FloatRange floatRange = new FloatRange(0, 1);
            Assert.AreEqual(0, floatRange.Min);
            Assert.AreEqual(1, floatRange.Max);
        }

        [Test]
        public void TestConstructorOutOfOrder()
        {
            FloatRange floatRange = new FloatRange(1, 0);
            Assert.AreEqual(0, floatRange.Min);
            Assert.AreEqual(1, floatRange.Max);
        }

        [Test]
        public void TestEqualsTrueDifferentObjects()
        {
            FloatRange floatRange1 = new FloatRange(1, 0);
            FloatRange floatRange2 = new FloatRange(1, 0);
            Assert.True(floatRange1 == floatRange2);
        }
    }
}