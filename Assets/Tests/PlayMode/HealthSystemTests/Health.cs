using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LightsOn.HealthSystem;

namespace HealthSystemTests {

public class HealthTests : PhotonTest {

    [UnityTest]
    public IEnumerator DamageWithoutDying() {
        GameObject obj = CreatePhotonGameObject();
        Health h = obj.AddComponent<Health>();

        h.maxHealth = 100;
        yield return null;

        h.Damage(10.0f, 0);
        yield return null;

        Assert.AreEqual(90, h.getHealth());
        Assert.IsNotNull(obj);
    }

    [UnityTest]
    public IEnumerator DamageExactAmountToDie() {
        GameObject obj = CreatePhotonGameObject();
        Health h = obj.AddComponent<Health>();

        h.maxHealth = 100;
        yield return null;

        h.Damage(100.0f, 0);
        yield return null;

        Assert.IsTrue(obj == null);
    }

    [UnityTest]
    public IEnumerator DamageExactlyToNearDeath() {
        GameObject obj = CreatePhotonGameObject();
        Health h = obj.AddComponent<Health>();

        h.maxHealth = 100;
        yield return null;

        h.Damage(99.0f, 0);
        yield return null;

        Assert.AreEqual(1, h.getHealth());
        Assert.IsNotNull(obj);
    }

    [UnityTest]
    public IEnumerator DamageToNearDeathOverFrames() {
        GameObject obj = CreatePhotonGameObject();
        Health h = obj.AddComponent<Health>();

        h.maxHealth = 100;
        yield return null;

        for (int i = 0; i < 99; i++) {
            h.Damage(1.0f, 0);
            yield return null;
        }

        Assert.AreEqual(1, h.getHealth());
        Assert.IsNotNull(obj);
    }

    [UnityTest]
    public IEnumerator DamageToDeathOverFrames() {
        GameObject obj = CreatePhotonGameObject();
        Health h = obj.AddComponent<Health>();

        h.maxHealth = 100;
        yield return null;

        for (int i = 0; i < 100; i++) {
            h.Damage(1.0f, 0);
            yield return null;
        }

        Assert.IsTrue(obj == null);
    }
}}