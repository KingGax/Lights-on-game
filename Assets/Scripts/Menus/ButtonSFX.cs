using UnityEngine;
using LightsOn.AudioSystem;

public class ButtonSFX : MonoBehaviour {

    public void mouseClick() {
        AudioManager.Instance.PlaySFX2D(SoundClips.Instance.SFXMenuClicks);
    }
}