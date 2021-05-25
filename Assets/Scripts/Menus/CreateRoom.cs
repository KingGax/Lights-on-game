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
    public int maxAllowedSpectators = 0;
    public Button playButton;
    private string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

    public void Awake() {
        TextMeshProUGUI t = playButton.GetComponentInChildren<TextMeshProUGUI>();
        t.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    }

    public void OnCLick_CreateRoom() {
        if(!PhotonNetwork.IsConnected)
            return;

        //Sets room options
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = System.Convert.ToByte(2 + maxAllowedSpectators);
        options.PublishUserId = true;
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable() { {"name", _roomName }, {"playerCount", 1}, { "highestActor", 2 } };
        string[] lobbyOptions = new string[3];
        lobbyOptions[0] = "name";
        lobbyOptions[1] = "playerCount"; //number of players, as opposed to spectators
        lobbyOptions[2] = "highestActor";
        options.CustomRoomPropertiesForLobby = lobbyOptions;
        options.CustomRoomProperties = properties;
        options.CleanupCacheOnLeave = false;

        string roomCode = System.Guid.NewGuid().ToString();
        if (!(string.IsNullOrEmpty(_roomName))) {
            PhotonNetwork.CreateRoom(roomCode, options);
        }
    }

    public override void OnCreatedRoom() {
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
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
            PhotonNetwork.LoadLevel("LobbyMenu");
        }
    }
}
