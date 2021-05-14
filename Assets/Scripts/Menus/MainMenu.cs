using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.AudioSystem;

public class MainMenu : MonoBehaviourPunCallbacks {
    

    private bool isConnecting;

    private bool hasJoinedRoom = false;
    public Animator transition;


    void Awake() {
        isConnecting = false;
        if(hasJoinedRoom){
            PhotonNetwork.LoadLevel("NameMenu");
        }
    }

    public void SetIsConnecting(bool state) {
        isConnecting = state;
    }
    public void PlayGame() {
        AudioManager.Instance.PlaySFX2D(SoundClips.Instance.SFXMenuClicks);
        transition.SetTrigger("Start");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public override void OnJoinedRoom() {
        AudioManager.Instance.PlaySFX2D(SoundClips.Instance.SFXMenuClicks);
        transition.SetTrigger("Start");
        hasJoinedRoom = true;
    }
}
