using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LightsOn.AudioSystem {
    [CreateAssetMenu(fileName="data", menuName="ScriptableObjects/SFX", order=1)]
    public class SFX : ScriptableObject {
        [SerializeField]
        protected List<AudioClip> clips;

        public AudioClip get(int i) {
            return clips[i % clips.Count];
        }
    }
}