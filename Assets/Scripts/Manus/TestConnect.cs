using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TestConnect : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Button createRoomButton;

    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
        createRoomButton.interactable = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        //To use that we need to manually configure networking client which looks painful
        //PhotonNetwork.ConnectToRegion("eu");
        PhotonNetwork.ConnectUsingSettings();

        
    }

    public override void OnConnectedToMaster(){
        Debug.Log("Connected to master.");
        PhotonNetwork.JoinLobby();
        
    }

    public override void OnJoinedLobby()
    {
        //ensures one cannot create a room before connection to server is established
        createRoomButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server: " + cause.ToString());
    }
}
