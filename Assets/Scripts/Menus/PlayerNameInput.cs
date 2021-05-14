using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.AudioSystem;

public class PlayerNameInput : MonoBehaviour {

    private string _playerName;

    public Button Enter;

    void Awake() {
        Enter.interactable = false;
    }

    public void SetPlayerName(string value) {
        if (!string.IsNullOrEmpty(value)) {
            _playerName = value;
            PhotonNetwork.NickName = _playerName;
            PlayerPrefs.SetString(_playerName,value);
            Enter.interactable = true;
        } else {
            Enter.interactable = false;
        }

    }

    public void OnPlayBTNPressed() {
        PhotonNetwork.LoadLevel("JoinRoomMenu");
    }

    public void OnBackBTNPressed() {
        PhotonNetwork.LoadLevel("StartMenu");
    }
}
