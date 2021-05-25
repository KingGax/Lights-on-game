using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;
using LightsOn.HealthSystem;

namespace HealthSystemTests {

public class EnemyHealthTests : PhotonTest {

    GameObject obj;
    EnemyHealth h;

    [UnitySetUp]
    public IEnumerator Setup() {
        obj = PhotonNetwork.Instantiate(
            "drone",
            new Vector3(0,0,0),
            Quaternion.identity,
            0
        );
        h = obj.GetComponent<EnemyHealth>();

        h.maxHealth = 100;
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown() {
        if (obj != null)
            PhotonNetwork.Destroy(obj);
        yield return null;
    }

    [UnityTest]
    public IEnumerator DamageWithoutDying() {
        h.Damage(10.0f, 0);
        yield return null;

        Assert.AreEqual(90, h.getHealth());
        Assert.IsFalse(obj == null);
    }

    [UnityTest]
    public IEnumerator DamageExactAmountToDie() {
        h.Damage(100.0f, 0);
        yield return null;

        Assert.IsTrue(obj == null);
    }

    [UnityTest]
    public IEnumerator DamageExactlyToNearDeath() {
        h.Damage(99.0f, 0);
        yield return null;

        Assert.AreEqual(1, h.getHealth());
        Assert.IsFalse(obj == null);
    }

    [UnityTest]
    public IEnumerator DamageToNearDeathOverFrames() {
        for (int i = 0; i < 99; i++) {
            h.Damage(1.0f, 0);
            yield return null;
        }

        Assert.AreEqual(1, h.getHealth());
        Assert.IsFalse(obj == null);
    }

    [UnityTest]
    public IEnumerator DamageToDeathOverFrames() {
        for (int i = 0; i < 100; i++) {
            h.Damage(1.0f, 0);
            yield return null;
        }

        Assert.IsTrue(obj == null);
    }
}}