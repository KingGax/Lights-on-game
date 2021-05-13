using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class VoiceMenu : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void setupMicrophoneUnity();

    public void OnRejectClicked() {
        SceneManager.LoadScene("StartMenu");
    }

    public void OnAcceptClicked() {
        setupMicrophoneUnity();
        //Debug.Log("Need to enable voice chat here");
    }
}