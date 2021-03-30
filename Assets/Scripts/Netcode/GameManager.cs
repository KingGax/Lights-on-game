using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPunCallbacks {
    private GameObject otherPlayerGO;
    private Player otherPlayer;
    private PlayerController otherPC;
    private PhotonView pv;
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;


    // Called when the local player left the room. We need to load the launcher scene.
    public override void OnLeftRoom() {
        GameObject destroyOnLoad = new GameObject();

        foreach (GameObject p in GlobalValues.Instance.players) {
            p.transform.SetParent(destroyOnLoad.transform);
        }
        GlobalValues.Instance.UIElements.gameObject.transform.SetParent(destroyOnLoad.transform);
        AudioManager.Instance.transform.SetParent(destroyOnLoad.transform);
        GlobalValues.Instance.gameObject.transform.SetParent(destroyOnLoad.transform);
        SceneManager.LoadScene(0);
    }

    private void Awake() {
        pv = gameObject.GetPhotonView();
    }

    void Start() {
        if (playerPrefab == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        } else {
            if (PlayerController.LocalPlayerInstance == null) {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                
                if (PhotonNetwork.LocalPlayer.ActorNumber < 3) {
                    Debug.Log("Actor number " + PhotonNetwork.LocalPlayer.ActorNumber);
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    DontDestroyOnLoad(GlobalValues.Instance.gameObject);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    if (PhotonNetwork.IsMasterClient) {
                        PhotonNetwork.Instantiate(this.playerPrefab.name, GlobalValues.Instance.p1spawn.position, Quaternion.identity, 0);
                        GlobalValues.Instance.navManager.SetPlayer(false);
                    }
                    else {
                        PhotonNetwork.Instantiate(this.playerPrefab.name, GlobalValues.Instance.p2Spawn.position, Quaternion.identity, 0);
                        GlobalValues.Instance.navManager.SetPlayer(true);
                    }
                }
                else {
                    pv.RPC("RequestOwnership", RpcTarget.MasterClient);
                }
            } else {
                if (PhotonNetwork.IsMasterClient) {
                    GlobalValues.Instance.players[0].transform.position = GlobalValues.Instance.p1spawn.position;
                    GlobalValues.Instance.navManager.SetPlayer(false);
                }
                else {
                    GlobalValues.Instance.players[0].transform.position = GlobalValues.Instance.p2Spawn.position;
                    GlobalValues.Instance.navManager.SetPlayer(true);
                }
                //Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    private void LoadArena() {
        if (!PhotonNetwork.IsMasterClient) {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Spawner");
    }

    public override void OnPlayerEnteredRoom(Player other) {
        otherPlayer = other;
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        Debug.Log("Entered room.");
        if (other.ActorNumber > 2) {
            
            if (GlobalValues.Instance.localPlayerInstance == GlobalValues.Instance.players[0]) {
                 otherPlayerGO = GlobalValues.Instance.players[1];
            }
            else {
                otherPlayerGO = GlobalValues.Instance.players[0];
            }
        }
        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            //LoadArena();
        }
        //Invoke("DelayOwnership", 2.0f);
    }

    [PunRPC]
    void RequestOwnership() {
        PhotonView otherPlayerView = otherPlayerGO.GetPhotonView();
        otherPlayerView.TransferOwnership(otherPlayer);
    }

    void DelayOwnership() {
        
    }

    /*private void DelayedTransfer() {
        otherPC.UpdateLocalPlayerInstance();
    }*/



    public override void OnPlayerLeftRoom(Player other) {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        Debug.Log(other.ActorNumber);
        GlobalValues.Instance.PlayerLeft();
        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            //LoadArena();
        }
    }
}