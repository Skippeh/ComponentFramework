using System;
using ComponentSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    internal class TestComponent : GameComponent
    {
        public string TestString { get; set; } = "default";

        private bool createCalled;
        private bool enableCalled;
        private bool disableCalled;
        private bool visibleCalled;
        private bool invisibleCalled;

        public override void OnCreate()
        {
            Console.WriteLine(String.Join("  ", "OnCreate Enabled:", Enabled, "Visible:", Visible));
            createCalled = true;
        }

        public override void OnDestroy()
        {
            Console.WriteLine(String.Join("  ", "OnDestroy Enabled:", Enabled, "Visible:", Visible));
        }

        public override void OnEnable()
        {
            Assert.IsTrue(createCalled, "OnEnable was called before OnCreate.");
            Assert.IsTrue(Enabled, "OnEnable was called but Enabled isn't true.");
            Console.WriteLine(String.Join("  ", "OnEnable Enabled:", Enabled, "Visible:", Visible));
            enableCalled = true;
        }

        public override void OnDisable()
        {
            Assert.IsTrue(enableCalled, "Disabled was called before OnEnable.");
            Assert.IsFalse(Enabled, "OnDisabled was called but Enabled isn't false.");
            Console.WriteLine(String.Join("  ", "OnDisable Enabled:", Enabled, "Visible:", Visible));
            disableCalled = true;
        }

        public override void OnVisible()
        {
            Assert.IsTrue(enableCalled, "OnVisible was called before OnEnable.");
            Assert.IsTrue(Visible, "OnVisible was called but Visible isn't true.");
            Console.WriteLine(String.Join("  ", "OnVisible Enabled:", Enabled, "Visible:", Visible));
            visibleCalled = true;
        }

        public override void OnInvisible()
        {
            Assert.IsTrue(visibleCalled, "OnInvisible was called before OnVisible.");
            Assert.IsFalse(Visible, "OnInvisible was called but Visible isn't false.");
            Console.WriteLine(String.Join("  ", "OnInvisible Enabled:", Enabled, "Visible:", Visible));
            invisibleCalled = true;
        }
    }
}