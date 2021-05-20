using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LightsOn.AudioSystem {
    public class AudioManager : MonoBehaviour {

        public AudioMixer mixer;

        private static AudioManager _instance;
        private AudioSource[]  audioSources = new AudioSource[2];
        private AudioSource SFX2D;
        public int playingTrack = 0;
        public int nextTrack = 0;
        private int freeAudioSource = 0;
        private double currentStartedTime = 0;
        private double nextStartTime = 0;
        private float volumeSFX = 1;

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

        private bool nextTrackScheduled = false;
        private bool nextTrackOnNext = false;

        private void ShortCircuite(double time) {
            Debug.Log("Looking to short circuit next track");
            Clip ac = audioClips[playingTrack];

            if (nextTrackOnNext && ac.endBehaviour == ClipEndBehaviour.NEXT) {
                return;
            } else {
                nextTrackOnNext = false;
            }

            if (ac.endBehaviour == ClipEndBehaviour.NEXT) {
                nextTrackOnNext = true;
                nextTrackScheduled = true;
                return;
            }

            nextStartTime = ac.getNextBeatAfterTime(
                (float) currentStartedTime,
                (float) time + 1.0f
            );
            // Reschedule current song to end
            audioSources[1-freeAudioSource].SetScheduledEndTime(nextStartTime);
            nextTrackScheduled = true;
        }

        public void Update() {
            double time = AudioSettings.dspTime;

            if (nextTrack != playingTrack && !nextTrackScheduled) {
                ShortCircuite(time);
            }

            if (time + 1.0f > nextStartTime) {
                Clip ac = getNextClip();
                if (ac == null) return;
                /*Debug.Log("Scheduling track " + nextTrack + " to play in 1 second"
                    + "\n\tstarting clip at: " + ac.start
                    + "\n\tclip ends at: " + ac.end
                );*/

                audioSources[freeAudioSource].clip = ac.audioClip;
                audioSources[freeAudioSource].time = ac.start;
                audioSources[freeAudioSource].PlayScheduled(nextStartTime);
                currentStartedTime = nextStartTime; // Update with next songs start time as scheduled
                nextStartTime += ac.length();
                audioSources[freeAudioSource].SetScheduledEndTime(nextStartTime);
                freeAudioSource = 1 - freeAudioSource;
                playingTrack = nextTrack;
                nextTrackScheduled = false;
                
                if (nextTrackOnNext) {
                    nextTrack++;
                }
            }
        }

        private Clip getNextClip() {
            if (nextTrack >= audioClips.Count) {
                return null;
            } else if (nextTrack == playingTrack) {
                Clip ac = audioClips[playingTrack];
                switch (ac.endBehaviour) {
                    case ClipEndBehaviour.NEXT:
                        nextTrack++;
                        /*Debug.Log("Advancing to track " + nextTrack
                            + " as track " + playingTrack + " has ended"
                        );*/
                        return getNextClip();
                    case ClipEndBehaviour.LOOP:
                    case ClipEndBehaviour.LOOPSHORT:
                    case ClipEndBehaviour.LOOPBEAT:
                        //Debug.Log("Scheduling to loop track " + playingTrack);
                        return ac;
                    default:
                    case ClipEndBehaviour.END:
                        return null;
                }
            } else {
                Debug.Log("Scheduling to move to next track, track " + nextTrack);
                Clip ac = audioClips[nextTrack];
                return ac;
            }
        }

        public void PlayNext() {
            // TODO: conditon where play next is called within a NEXT section
            Debug.Log("Play Next");
            nextTrack++;
        }

        private void generateAudioSources() {
            AudioMixerGroup[] outs = mixer.FindMatchingGroups("Master/Music");
            GameObject child = new GameObject("AudioPlayer");
            child.transform.parent = gameObject.transform;
            SFX2D = child.AddComponent<AudioSource>();
            SFX2D.outputAudioMixerGroup = mixer.FindMatchingGroups("Master/SFX2D")[0];
            for (int i = 0; i < audioSources.Length; i++) {
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

            AudioSource.PlayClipAtPoint(clip.get(sfxVersion), pos, volumeSFX);
        }

        public void PlaySFX2D(SFX clip) {
            SFX2D.PlayOneShot(clip.get(0), volumeSFX);
        }
    }
}