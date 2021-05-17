using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using LightsOn.AudioSystem;

public class VoiceMenu : MonoBehaviour {


    [DllImport("__Internal")]
    private static extern void setupMicrophoneUnity();

    public void OnRejectClicked() {
        GlobalValues.Instance.micEnabled = false;
        GlobalValues.Instance.micEditable = false;
        AudioManager.Instance.PlayNext();
        Invoke("ChangeScene", 1);
    }

    public void OnAcceptClicked() {
        #if !UNITY_EDITOR
            #if UNITY_WEBGL
                setupMicrophoneUnity();
            #endif
        #endif
        AudioManager.Instance.PlayNext();
        Invoke("ChangeScene", 1);
        //Debug.Log("Need to enable voice chat here");
        //AudioManager.Instance.PlayNext();
    }

    private void ChangeScene() {
        SceneManager.LoadScene("StartMenu");
    }
}