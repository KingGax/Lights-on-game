using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Composition : MonoBehaviour {

    public AudioClip musicClip;
    public List<double> beatstamps;
    public double bpm;

    public int section = 2;

    public double getSectionLength() {
        double delta = beatstamps[section + 1] - beatstamps[section];
        return delta * 60.0f / bpm;
    }

    public double getSectionStartTime() {
        return beatstamps[section] * 60.0f / bpm;
    }
}