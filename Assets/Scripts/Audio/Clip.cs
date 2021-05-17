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
        public float[] shortCicuiteLocations;

        public float length() {
            return end - start;
        }

        public float getNextBeatAfterTime(float startT, float t) {
            switch (endBehaviour) {
                case ClipEndBehaviour.LOOP:
                case ClipEndBehaviour.NEXT:
                    // No short circuiting
                    Debug.Log("LOOP/NEXT end calced: " + startT + " " + t
                        + " " + start + " " + end + " " + (startT + end)
                    );
                    return startT + end;
                case ClipEndBehaviour.LOOPSHORT:
                    foreach (float l in shortCicuiteLocations) {
                        if (t < startT + l) {
                            Debug.Log("Hi" + t + " " + startT + " " + l);
                            return startT + l;
                        }
                    }
                    return startT + end;
                case ClipEndBehaviour.END:
                case ClipEndBehaviour.LOOPBEAT:
                default:
                    float tbeats = Mathf.Ceil(t * bpm / 60.0f);
                    return tbeats * 60.0f/bpm;
            }
        }
    }

    public enum ClipEndBehaviour {
        END,
        LOOP,
        LOOPSHORT,
        NEXT,
        LOOPBEAT
    }
}