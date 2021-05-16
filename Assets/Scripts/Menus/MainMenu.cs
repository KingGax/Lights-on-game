using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.AudioSystem;

public class MainMenu : MonoBehaviourPunCallbacks {
    

    private bool isConnecting;
    private bool hasJoinedRoom = false;


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
