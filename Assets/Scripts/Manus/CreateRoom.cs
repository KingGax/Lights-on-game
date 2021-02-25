using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _roomName;

    [SerializeField]



    public void OnCLick_CreateRoom()
    {
        if(!PhotonNetwork.IsConnected)
            return;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        options.PublishUserId = true;
        if (!(string.IsNullOrEmpty(_roomName))){
            PhotonNetwork.CreateRoom(_roomName, options);
        }
        else {
            Debug.Log("Empty room name");
        }

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Successfully created room.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed: " + message);
    }

    public void SetRoomName(string value){
        if (!string.IsNullOrEmpty(value)) {
            _roomName = value;
            PlayerPrefs.SetString(_roomName,value);
        }
    }

    

    public override void OnJoinedRoom() {
            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
                PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
}
