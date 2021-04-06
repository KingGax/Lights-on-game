using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LightsOn.HealthSystem;

namespace Tests {
    public class NewTestScript {

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses() {
            var gameObject = new GameObject();
            Health h = gameObject.AddComponent<Health>();
            h.maxHealth = 100;
            h.Damage(100, 0);
            yield return null;
            Assert.IsNull(gameObject);
        }
    }
}
