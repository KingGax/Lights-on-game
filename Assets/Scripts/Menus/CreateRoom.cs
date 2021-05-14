using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class CreateRoom : MonoBehaviourPunCallbacks {

    [SerializeField]
    private string _roomName;
    
    [DllImport("__Internal")]
    private static extern void setupVoiceChatUnity(string username, string role);
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

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = System.Convert.ToByte(2 + maxAllowedSpectators);
        options.PublishUserId = true;
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable() { {"name", _roomName }, {"playerCount", 1}};
        string[] lobbyOptions = new string[2];
        lobbyOptions[0] = "name";
        lobbyOptions[1] = "playerCount"; //number of players, as opposed to spectators
        options.CustomRoomPropertiesForLobby = lobbyOptions;
        options.CustomRoomProperties = properties;
        options.CleanupCacheOnLeave = false;

        //Code for generating random string. Opted for using GUID instead
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
        #if !UNITY_EDITOR
            #if UNITY_WEBGL
            if(GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceChatEnabled) {
                setupVoiceChatUnity(PhotonNetwork.CurrentRoom.Name, "master");
            }
            #endif
        #endif
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
            PhotonNetwork.LoadLevel("LobbyMenu");
        }
    }
}
