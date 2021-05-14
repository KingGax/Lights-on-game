using UnityEngine;

namespace LightsOn.AudioSystem {
    [CreateAssetMenu(fileName="data", menuName="ScriptableObjects/AudioClip", order=1)]
    public class Clip : ScriptableObject {
        public AudioClip audioClip;
        public float bpm;
        // Time stamps
        public float start;
        public float end;
        public ClipEndBehaviour endBehaviour;

        public float length() {
            return end - start;
        }

        public float getNextBeatAfterTime(float t) {
            float tbeats = Mathf.Ceil(t * bpm / 60.0f);
            return tbeats * 60.0f/bpm;
        }
    }

    public enum ClipEndBehaviour {
        END,
        LOOP,
        NEXT
    }
}