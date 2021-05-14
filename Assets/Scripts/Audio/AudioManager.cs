using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LightsOn.AudioSystem {
    public class AudioManager : MonoBehaviour {

        public AudioMixer mixer;

        private static AudioManager _instance;
        private AudioSource[]  audioSources = new AudioSource[2];
        private int playingTrack = 0;
        private int nextTrack = 0;
        private int freeAudioSource = 0;
        private double nextStartTime = 0;

        public static AudioManager Instance { get { return _instance; } }

        [SerializeField]
        private List<Clip> audioClips;

        public void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
                _instance = this;
                generateAudioSources();
            }
        }

        public void Update() {
            double time = AudioSettings.dspTime;
            if (nextTrack != playingTrack) {
                Clip ac = getNextClip();
                nextStartTime = ac.getNextBeatAfterTime((float)time + 1.0f);
                playingTrack++;
                // Reschedule current song to end
                audioSources[1-freeAudioSource].SetScheduledEndTime(nextStartTime);
            }

            if (time + 1.0f > nextStartTime) {
                Clip ac = getNextClip();
                if (ac == null) return;

                audioSources[freeAudioSource].clip = ac.audioClip;
                audioSources[freeAudioSource].time = ac.start;
                audioSources[freeAudioSource].PlayScheduled(nextStartTime);
                nextStartTime += ac.length();
                audioSources[freeAudioSource].SetScheduledEndTime(nextStartTime);
                freeAudioSource = 1 - freeAudioSource;
            }
        }

        private Clip getNextClip() {
            if (nextTrack >= audioClips.Count) {
                return null;
            } else if (nextTrack == playingTrack) {
                Clip ac = audioClips[nextTrack];
                switch (ac.endBehaviour) {
                    case ClipEndBehaviour.NEXT:
                        nextTrack++;
                        return getNextClip();
                    case ClipEndBehaviour.LOOP:
                        return ac;
                    default:
                    case ClipEndBehaviour.END:
                        return null;
                }
            } else {
                Clip ac = audioClips[nextTrack];
                return ac;
            }
        }

        public void PlayNext() {
            nextTrack++;
            // Deal with short cicuiting current song
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

        // SFX
        private IDictionary<int, IDictionary<int, int>> playCount = new Dictionary<int, IDictionary<int, int>>();

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