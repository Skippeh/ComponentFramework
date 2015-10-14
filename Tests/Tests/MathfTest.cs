using System;
using ComponentSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Tests.Tests
{
    [TestClass]
    public class MathfTest
    {
        [TestMethod]
        public void TestMathf()
        {
            // Test Mathf.ToDirection
            Vector2 direction = Mathf.ToDirection(MathHelper.ToRadians(90));
            Assert.IsTrue(Math.Abs(direction.X - 1d) <= 0.0001d && Math.Abs(direction.Y) <= 0.0001d, "ToDirection test expected Vec2(1, 0).");
            
            // Test Mathf.SmoothLerp010
            Assert.AreEqual(2f, Mathf.SmoothLerp010(1f, 2f, 0.5f), "SmoothLerp010(1, 2, 0.5) expected 2.");
        }
    }
}