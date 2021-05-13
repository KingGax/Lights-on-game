using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class PhotonConnector : MonoBehaviourPunCallbacks {

    private int tries = 0;

    void Start() {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
        //To use that we need to manually configure networking client which looks painful
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster(){
        Debug.Log("Connected to master.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        PhotonNetwork.LoadLevel("NameMenu");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("Disconnected from server: " + cause.ToString());
        tries++;
        if (tries == 1) {
            Debug.Log("Trying to connect to Russia");
            PhotonNetwork.ConnectToRegion("ru");
        } else if (tries == 2) {
            Debug.Log("Trying to connect to USA");
            PhotonNetwork.ConnectToRegion("us");
        }
    }
}
