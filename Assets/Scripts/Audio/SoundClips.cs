using UnityEngine;

public class SoundClips : MonoBehaviour {
    
    private static SoundClips _instance;
    public static SoundClips Instance { get { return _instance; } }

    public AudioClip Overworld;

    public AudioClip SFXLazer;
    public AudioClip SFXShoot;
    public AudioClip SFXDoorOpen;
    public AudioClip SFXLightChange;
    public AudioClip SFXKill;

    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}