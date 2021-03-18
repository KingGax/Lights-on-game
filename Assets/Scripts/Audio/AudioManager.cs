using UnityEngine;

public class AudioManager : MonoBehaviour {
    
    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }

    public static void PlaySFX(AudioClip clip, Vector3 pos) {
        AudioSource.PlayClipAtPoint(clip, pos);
    }

    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}