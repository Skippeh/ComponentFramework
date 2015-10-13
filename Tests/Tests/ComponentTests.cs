using System.Linq;
using ComponentSystem;
using ComponentSystem.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Tests
{
    [TestClass]
    public class ComponentsTest
    {
        [TestMethod]
        public void TestComponents()
        {
            var testGame = new TestGame();
            testGame.OnInitialized += game =>
            {
                var manager = game.GameObjectManager;

                // Expect there to be 0 game objects.
                Assert.AreEqual(0, manager.GameObjects.Count, "Expected 0 root objects.");

                var go = game.AddGameObject();
                var child = go.AddObject();

                // Expect there to be 1 root game object and 1 child in go.
                Assert.AreEqual(1, manager.GameObjects.Count, "Expected 1 root object.");
                Assert.AreEqual(1, go.Children.Count, "Expected 1 child object.");

                // Expect go's name to be the name of the class.
                Assert.AreEqual(typeof(GameObject).Name, go.Name, "Expected root object's name to be 'GameObject'.");
                Assert.AreEqual(typeof(GameObject).Name, child.Name, "Expected child's name to be 'GameObject'.");

                // Expect object's sibling count to be 1.
                Assert.AreEqual(1, go.Siblings.Count, "Expected root object's siblings count to be 1.");
                Assert.AreEqual(1, child.Siblings.Count, "Expected child object's siblings count to be 1.");

                var child2 = go.AddObject();

                // Expect root object's sibling count to be 2.
                Assert.AreEqual(1, go.Siblings.Count, "Expected root object's siblings count to be 1.");
                Assert.AreEqual(2, child.Siblings.Count, "Expected child object's siblings count to be 2.");
                Assert.AreEqual(2, go.Children.Count, "Expected root object's children count to be 2.");

                // Expect object component count to be 1 and for it to have a Transform component.
                Assert.AreEqual(1, go.Components.Count, "Expected object's Component count to be 1.");
                Assert.AreEqual(typeof(Transform), go.GetComponent<Transform>()?.GetType(), "Expected object to have a Transform component.");

                // Expect object to be enabled and visible.
                Assert.AreEqual(true, go.Enabled, "Expected object to be Enabled.");
                Assert.AreEqual(true, go.Visible, "Expected object to be visible.");

                // Expect every component to be enabled.
                Assert.IsTrue(go.Components.All(component => component.Value.Enabled), "Expected all components to be enabled.");

                go.Enabled = false;

                // Expect every component to be disabled.
                Assert.IsTrue(go.Components.All(comp => !comp.Value.Enabled), "Expected all components to be disabled.");

                go.Enabled = true;

                // Expect every component to be enabled.
                Assert.IsTrue(go.Components.All(comp => comp.Value.Enabled), "Expected all components to be enabled.");
                
                child.Destroy();

                // Expect there to be 1 child object and for child to be destroyed.
                Assert.AreEqual(1, go.Children.Count, "Expected 1 child object.");
                Assert.AreEqual(true, child.Destroyed, "Expected child to be destroyed.");
                Assert.AreEqual(0, child.Components.Count, "Expected destroyed object to have 0 components.");

                go.Destroy();

                // Expect there to be 0 objects and for Object to be destroyed.
                Assert.AreEqual(0, manager.GameObjects.Count, "Expected 0 root objects.");
                Assert.AreEqual(true, go.Destroyed, "Expected object to be destroyed.");

                go = game.AddGameObject();
                var clone = go.Clone();

                // Expect clones to not equal the original object.
                Assert.AreNotEqual(go, clone, "Expected clone to be separate instance");
                Assert.IsTrue(go.Components.Values.All(comp => !clone.Components.ContainsValue(comp)), "Expected cloned components to be separate instances from original components.");
                Assert.IsTrue(go.Children.All(_child => !clone.Children.Contains(_child)), "Expected cloned children to be separate instances from original children.");
                
                game.GameObjectManager.Destroy();

                // Expect there to be 0 objects.
                Assert.AreEqual(0, game.GameObjectManager.GameObjects.Count, "Expected total root objects count to be 0.");
            };
            testGame.RunOneFrame();
        }
    }
}