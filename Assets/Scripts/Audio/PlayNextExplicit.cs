using UnityEngine;
using LightsOn.AudioSystem;

public class PlayNextExplicit : MonoBehaviour {
    public void Awake() {
        AudioManager.Instance.PlayNext();
    }
}