using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Photon.Pun;
using LightsOn.AudioSystem;

public class MainMenu : MonoBehaviourPunCallbacks {
    

    private bool isConnecting;
    private bool hasJoinedRoom = false;
    public AudioMixer mixer;
    float initialVolume = 0.5f;


    void Awake() {
        isConnecting = false;
        if(hasJoinedRoom){
            PhotonNetwork.LoadLevel("NameMenu");
        }
        mixer.SetFloat("MasterVolume", Mathf.Log10(initialVolume) * 20);
    }

    public void SetIsConnecting(bool state) {
        isConnecting = state;
    }
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public override void OnJoinedRoom() {
        hasJoinedRoom = true;
    }
    public void loadOptions() {
        SceneManager.LoadScene("OptionsMenu");
    }
    public void loadOptionsWrapper() {
        Invoke("loadOptions", 1);
    }
    
}
