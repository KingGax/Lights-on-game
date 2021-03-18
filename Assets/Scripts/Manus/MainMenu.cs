using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenu : MonoBehaviourPunCallbacks {
    

    private bool isConnecting;

    private bool hasJoinedRoom = false;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject playMenu;

    void Awake() {
        isConnecting = false;
        if(hasJoinedRoom){
            mainMenu.SetActive(false);
            playMenu.SetActive(true);
        }
        else {
            mainMenu.SetActive(true);
            playMenu.SetActive(false);
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
}
