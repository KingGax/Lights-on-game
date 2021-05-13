using UnityEngine;
using UnityEngine.SceneManagement;

public class VoiceMenu : MonoBehaviour {

    public void OnRejectClicked() {
        SceneManager.LoadScene("StartMenu");
    }

    public void OnAcceptClicked() {
        Debug.Log("Need to enable voice chat here");
    }
}