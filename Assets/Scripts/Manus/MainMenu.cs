using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenu : MonoBehaviour {
    
    private string _playerName;

    private bool isConnecting;

    void Awake() {
        isConnecting = false;
    }

    public void SetIsConnecting(bool state) {
        isConnecting = state;
    }
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SetPlayerName(string value){
        if (!string.IsNullOrEmpty(value)) {
            _playerName = value;
            PhotonNetwork.NickName = _playerName;
            PlayerPrefs.SetString(_playerName,value);
        }
    }
}
