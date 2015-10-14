using System;
using ComponentSystem;

namespace Tests
{
    internal class TestComponent : GameComponent
    {
        public string TestString { get; set; } = "default";

        public override void OnCreate()
        {
            Console.WriteLine(String.Join("  ", "OnCreate Enabled:", Enabled, "Visible:", Visible));
        }

        public override void OnDestroy()
        {
            Console.WriteLine(String.Join("  ", "OnDestroy Enabled:", Enabled, "Visible:", Visible));
        }

        public override void OnEnable()
        {
            Console.WriteLine(String.Join("  ", "OnEnable Enabled:", Enabled, "Visible:", Visible));
        }

        public override void OnDisable()
        {
            Console.WriteLine(String.Join("  ", "OnDisable Enabled:", Enabled, "Visible:", Visible));
        }

        public override void OnVisible()
        {
            Console.WriteLine(String.Join("  ", "OnVisible Enabled:", Enabled, "Visible:", Visible));
        }

        public override void OnInvisible()
        {
            Console.WriteLine(String.Join("  ", "OnInvisible Enabled:", Enabled, "Visible:", Visible));
        }
    }
}