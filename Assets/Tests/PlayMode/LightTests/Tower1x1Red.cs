using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;
using LightsOn.LightingSystem;

namespace LightSystemTests {

    public class Tower1x1RedTests : PhotonTest {

        LightableObstacle tower;
        GameObject obj;
        Lanturn l;

        [UnitySetUp]
        public IEnumerator Setup() {
            obj = CreatePhotonGameObject();
            obj.layer = LayerMask.NameToLayer("LightingHitboxes");

            // Need to add a RB so that the collider triggers
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false;

            l = obj.AddComponent<Lanturn>();
            Light light = obj.AddComponent<Light>();
            l.light = light;
            light.range = 10;
            l.SetColour(LightColour.Red);
            obj.GetComponent<SphereCollider>().isTrigger = true;

            tower = GameObject.Find("/1x1_tower/LightingHitboxes")
                .GetComponent<LightableObstacle>();
            tower.SetColour(LightColour.Red);

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown() {
            if (obj != null) {
                obj.transform.position = new Vector3(100.0f, 100.0f, 100.0f);
                yield return new WaitForSeconds(1);
                PhotonNetwork.Destroy(obj);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator DoesNotAffectTowerRed() {
            obj.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Red);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator DoesNotAffectTowerGreen() {
            obj.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Green);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator DoesNotAffectTowerBlue() {
            obj.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Blue);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator DoesNotAffectTowerCyan() {
            obj.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Cyan);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator DoesNotAffectTowerYellow() {
            obj.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Yellow);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator DoesNotAffectTowerMagenta() {
            obj.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Magenta);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator InRangeOfTowerRed() {
            obj.transform.position = new Vector3(-8.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Red);
            yield return new WaitForSeconds(1);
            Assert.IsTrue(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator InRangeOfTowerGreen() {
            obj.transform.position = new Vector3(-8.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Green);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator InRangeOfTowerBlue() {
            obj.transform.position = new Vector3(-8.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Blue);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator InRangeOfTowerCyan() {
            obj.transform.position = new Vector3(-8.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Cyan);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator InRangeOfTowerYellow() {
            obj.transform.position = new Vector3(-8.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Yellow);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }

        [UnityTest]
        public IEnumerator InRangeOfTowerMagenta() {
            obj.transform.position = new Vector3(-8.0f, 0.0f, 0.0f);
            l.SetColour(LightColour.Magenta);
            yield return new WaitForSeconds(1);
            Assert.IsFalse(tower.isHidden);
        }
    }
}