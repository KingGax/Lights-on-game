using UnityEngine;
using LightsOn.AudioSystem;

public class ButtonSFX : MonoBehaviour {

    public void mouseClick() {
        AudioManager.Instance.PlaySFX(
            SoundClips.Instance.SFXMenuClicks,
            transform.position,
            gameObject
        );
    }
}