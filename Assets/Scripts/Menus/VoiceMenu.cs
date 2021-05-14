using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using LightsOn.AudioSystem;

public class VoiceMenu : MonoBehaviour {

    public Animator transition;

    [DllImport("__Internal")]
    private static extern void setupMicrophoneUnity();

    public void OnRejectClicked() {
        AudioManager.Instance.PlayNext();
        AudioManager.Instance.PlaySFX2D(SoundClips.Instance.SFXMenuClicks);
        transition.SetTrigger("Start");
        Invoke("ChangeScene", 1);
    }

    public void OnAcceptClicked() {
        AudioManager.Instance.PlaySFX2D(SoundClips.Instance.SFXMenuClicks);
        setupMicrophoneUnity();
        //Debug.Log("Need to enable voice chat here");
        //AudioManager.Instance.PlayNext();
    }

    private void ChangeScene() {
        SceneManager.LoadScene("StartMenu");
    }
}