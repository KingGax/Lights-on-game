using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace LightsOn.AudioSystem {
    [CreateAssetMenu(fileName="data", menuName="ScriptableObjects/Composition", order=1)]
    public class Composition : ScriptableObject {

        public AudioClip musicClip;
        public List<double> beatstamps;
        public double bpm;
        public int section = 0;

        public double getSectionLength() {
            double delta = beatstamps[section + 1] - beatstamps[section];
            return delta * 60.0f / bpm;
        }

        public double getSectionStartTime() {
            return beatstamps[section] * 60.0f / bpm;
        }

        public bool playNextSection() {
            if (section == beatstamps.Count - 2) {
                return false;
            } else {
                section++;
                return true;
            }
        }
    }
}
