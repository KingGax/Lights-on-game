using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using LightsOn.AudioSystem;

public class VoiceMenu : MonoBehaviour {


    [DllImport("__Internal")]
    private static extern void setupMicrophoneUnity();

    public void OnRejectClicked() {
        AudioManager.Instance.PlayNext();
        Invoke("ChangeScene", 1);
    }

    public void OnAcceptClicked() {
        setupMicrophoneUnity();
        //Debug.Log("Need to enable voice chat here");
        //AudioManager.Instance.PlayNext();
    }

    private void ChangeScene() {
        SceneManager.LoadScene("StartMenu");
    }
}