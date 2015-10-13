using System;
using ComponentSystem;

namespace Tests
{
    public class TestGame : ComponentBasedGame
    {
        public Action<TestGame> OnInitialized;

        protected override void Initialize()
        {
            base.Initialize();
            OnInitialized?.Invoke(this);
        }
    }
}