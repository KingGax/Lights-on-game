using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using LightsOn.AudioSystem;

namespace AudioTests {
    public class AudioTests {

        private GameObject audioMgr;
        private GameObject listner;
/*
 * Audio testing cannot be automated due to
 * A race confition caused by the yeild returns
 * Not affecting the audio thread
 *
        [UnitySetUp]
        public IEnumerator Setup() {
            audioMgr = Object.Instantiate(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/LevelComponents/AudioManager.prefab"
                )
            );
            listner = new GameObject();
            listner.AddComponent<AudioListener>();
            yield return new EnterPlayMode();
            yield return new WaitForSeconds(0.5f);
        }

        [UnityTearDown]
        public IEnumerator TearDown() {
            Object.Destroy(audioMgr);
            Object.Destroy(listner);
            yield return new ExitPlayMode();
        }


        [UnityTest]
        public IEnumerator OneClipPlaysOnAwake() {
            AudioSource[] srcs = audioMgr.GetComponentsInChildren<AudioSource>();
            int numplaying = 0;

            yield return new WaitForSeconds(0.5f);
            foreach (AudioSource s in srcs) {
                if (s.isPlaying)
                    numplaying++;
            }
            Assert.AreEqual(1, numplaying);
        }

        [UnityTest]
        public IEnumerator PlayNextSwapsSrc() {
            int idx = AudioManager.Instance.freeAudioSource;
            AudioManager.Instance.PlayNext();
            yield return null;
            Assert.AreNotEqual(AudioManager.Instance.freeAudioSource, idx);
        }

        [UnityTest]
        public IEnumerator PlayNextChangesClip() {
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.PlayNext();
            yield return new WaitForSeconds(1.5f);
            Assert.AreEqual(1, AudioManager.Instance.playingTrack);
        }

        [UnityTest]
        public IEnumerator PlayNextOnlyOnePlaying() {
            AudioSource[] srcs = audioMgr.GetComponentsInChildren<AudioSource>();
            int idx = 0;

            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.PlayNext();
            yield return new WaitForSeconds(1.5f);

            for (int i = 0; i < srcs.Length; i++) {
                if (srcs[i].isPlaying) {
                    idx++;
                }
            }

            Assert.AreEqual(1, idx);
        }

        [UnityTest]
        public IEnumerator Loops() {
            AudioSource[] srcs = audioMgr.GetComponentsInChildren<AudioSource>();
            int idx = 0;

            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < srcs.Length; i++) {
                if (srcs[i].isPlaying) {
                    idx = i;
                }
            }

            yield return new WaitForSeconds(7f);
            for (int i = 0; i < srcs.Length; i++) {
                if (srcs[i].isPlaying) {
                    Assert.IsTrue(idx != i);
                }
            }
        }

        [UnityTest]
        public IEnumerator LoopOnlyOnePlaying() {
            AudioSource[] srcs = audioMgr.GetComponentsInChildren<AudioSource>();
            int idx = 0;

            yield return new WaitForSeconds(6.5f);
            for (int i = 0; i < srcs.Length; i++) {
                if (srcs[i].isPlaying) {
                    idx++;
                }
            }

            Assert.AreEqual(1, idx);
        }
*/
    }
}
