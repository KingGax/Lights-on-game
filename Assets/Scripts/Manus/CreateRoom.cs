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
    private string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

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
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable() { {"name", _roomName }};
        //properties.Add("name", _roomName);
        string[] lobbyOptions = new string[1];
        lobbyOptions[0] = "name";
        options.CustomRoomPropertiesForLobby = lobbyOptions;
        options.CustomRoomProperties = properties;
        // int charAmount = 6;
        // string roomCode = "";
        // PhotonNetwork.
        // do
        // {
        //     for(int i=0; i<charAmount; i++)
        //     {
        //         roomCode += glyphs[Random.Range(0, glyphs.Length)];
        //     }
        // } while()
        string roomCode = System.Guid.NewGuid().ToString();
        if (!(string.IsNullOrEmpty(_roomName))) {
            PhotonNetwork.CreateRoom(roomCode, options);
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
