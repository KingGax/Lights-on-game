using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public AudioMixer mixer;

    private static AudioManager _instance;
    private AudioSource[]  audioSources = new AudioSource[2];
    public List<Composition> tracks;
    private int trackIndex = -1;
    private int freeAudioSource = 0;
    private double nextStartTime = 0;
    private bool running = false;

    public static AudioManager Instance { get { return _instance; } }

    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
            generateAudioSources();
            PlayNext();
        }
    }

    public void Update() {
        if (!running) return;

        double time = AudioSettings.dspTime;
        if (time + 1.0f > nextStartTime) {
            audioSources[freeAudioSource].clip = getNextClip();
            audioSources[freeAudioSource].time = (float) getNextSectionTime();
            audioSources[freeAudioSource].PlayScheduled(nextStartTime);
            nextStartTime += getSectionLength();
            audioSources[freeAudioSource].SetScheduledEndTime(nextStartTime);
            freeAudioSource = 1 - freeAudioSource;
        }
    }

    private double getNextSectionTime() {
        return tracks[trackIndex].getSectionStartTime();
    }

    private double getSectionLength() {
        return tracks[trackIndex].getSectionLength();
    }

    private AudioClip getNextClip() {
        if (tracks[trackIndex].section == tracks[trackIndex].beatstamps.Count) {
            trackIndex++;
        }

        return tracks[trackIndex].musicClip;
    }

    public void PlayNext() {
        if (trackIndex < tracks.Count) {
            trackIndex++;
            running = true;
        }
    }

    private void generateAudioSources() {
        AudioMixerGroup[] outs = mixer.FindMatchingGroups("Master/Music");
        for (int i = 0; i < audioSources.Length; i++) {
            GameObject child = new GameObject("AudioPlayer");
            child.transform.parent = gameObject.transform;
            audioSources[i] = child.AddComponent<AudioSource>();
            audioSources[i].outputAudioMixerGroup = outs[i+1];
        }
    }

    public static void PlaySFX(AudioClip clip, Vector3 pos) {
        AudioSource.PlayClipAtPoint(clip, pos);
    }
}
