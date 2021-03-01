using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class CreateRoom : MonoBehaviourPunCallbacks {

    [SerializeField]
    private string _roomName;
    public Button playButton;

    public void Awake() {
        TextMeshProUGUI t = playButton.GetComponentInChildren<TextMeshProUGUI>();
        t.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    }

    public void OnCLick_CreateRoom() {
        if(!PhotonNetwork.IsConnected)
            return;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        options.PublishUserId = true;
        if (!(string.IsNullOrEmpty(_roomName))) {
            PhotonNetwork.CreateRoom(_roomName, options);
        } else {
            Debug.Log("Empty room name");
        }
    }

    public override void OnCreatedRoom() {
        Debug.Log("Successfully created room.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Room creation failed: " + message);
    }

    public void SetRoomName(string value) {
        if (!string.IsNullOrEmpty(value)) {
            _roomName = value;
            PlayerPrefs.SetString(_roomName,value);
            playButton.interactable = true;
            TextMeshProUGUI t = playButton.GetComponentInChildren<TextMeshProUGUI>();
            t.color = Color.white;
        } else {
            playButton.interactable = false;
            TextMeshProUGUI t = playButton.GetComponentInChildren<TextMeshProUGUI>();
            t.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
    }

    public override void OnJoinedRoom() {
        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
