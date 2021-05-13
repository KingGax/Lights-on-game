using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LightsOn.AudioSystem {
    public class AudioManager : MonoBehaviour {

        public AudioMixer mixer;

        private static AudioManager _instance;
        private AudioSource[]  audioSources = new AudioSource[2];
        public List<Composition> tracks;
        private int trackIndex = -1;
        private int freeAudioSource = 0;
        private double nextStartTime = 0;
        private bool running = false;

        private double nextTransitionEnd = 0;
        private float transitionLength = 3.0f;

        // SFX
        private IDictionary<int, IDictionary<int, int>> playCount = new Dictionary<int, IDictionary<int, int>>();

        public static AudioManager Instance { get { return _instance; } }

        public void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
                _instance = this;
                generateAudioSources();
                foreach (Composition track in tracks) {
                    track.section = -1;
                }
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

           /* if (time > nextTransitionEnd) {
                nextTransitionEnd = nextStartTime;
                mixer.SetFloat("Track1HighPass", 10.0f);
                mixer.SetFloat("Track2HighPass", 10.0f);
            } else if (time + transitionLength > nextTransitionEnd) {
                float delta = Mathf.Sin(Mathf.PI/2 * (float)(nextTransitionEnd - time) / transitionLength);
                float pctTransition = 1.0f - Mathf.Min(1.0f, Mathf.Max(delta, 0.0f));
                float frq = 10.0f + 2990.0f * pctTransition;
                mixer.SetFloat("Track1HighPass", frq);
                mixer.SetFloat("Track2HighPass", frq);
            }*/
        }

        private double getNextSectionTime() {
            return tracks[trackIndex].getSectionStartTime();
        }

        private double getSectionLength() {
            return tracks[trackIndex].getSectionLength();
        }

        private AudioClip getNextClip() {
            return tracks[trackIndex].musicClip;
        }

        public void PlayNext() {
            if(trackIndex == -1){
                trackIndex++;
                running = true;
            }
            if(!tracks[trackIndex].playNextSection()){
                if (trackIndex < tracks.Count - 1) {
                    trackIndex++;
                    running = true;
                }
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

        public void PlaySFX(SFX clip, Vector3 pos, GameObject obj) {
            int sfxVersion = 0;
            if (playCount.ContainsKey(obj.GetInstanceID())) {
                IDictionary<int, int> sfxCount = playCount[obj.GetInstanceID()];
                if (sfxCount.ContainsKey(clip.GetInstanceID())) {
                    sfxVersion = sfxCount[clip.GetInstanceID()];
                    sfxCount[clip.GetInstanceID()] += 1;
                } else {
                    sfxCount.Add(clip.GetInstanceID(), 1);
                }
            } else {
                IDictionary<int, int> sfxCount = new Dictionary<int, int>();
                sfxCount.Add(clip.GetInstanceID(), 1);
                playCount.Add(obj.GetInstanceID(), sfxCount);
            }

            AudioSource.PlayClipAtPoint(clip.get(sfxVersion), pos);
        }
    }
}