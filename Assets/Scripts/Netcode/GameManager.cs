using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviourPunCallbacks {
    private GameObject otherPlayerGO;
    private Player otherPlayer;
    private PlayerController otherPC;
    private PhotonView pv;
    private RejoinTextUI rejoinText;
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    [DllImport("__Internal")]
    private static extern void disconnectVoiceChatUnity();

    [DllImport("__Internal")]
    private static extern void setupVoiceChatUnity(string roomName, string role);

    // Called when the local player left the room. We need to load the launcher scene.
    public override void OnLeftRoom() {
        foreach (GameObject p in GlobalValues.Instance.players) {
            //p.transform.SetParent(destroyOnLoad.transform);
            Destroy(p);
        }
        Destroy(GlobalValues.Instance.UIElements.gameObject);
        //GlobalValues.Instance.UIElements.gameObject.transform.SetParent(destroyOnLoad.transform);
        //Destroy(AudioManager.Instance.gameObject);
        //AudioManager.Instance.transform.SetParent(destroyOnLoad.transform);
        //GlobalValues.Instance.gameObject.transform.SetParent(destroyOnLoad.transform);
        Destroy(GlobalValues.Instance.gameObject);
        SceneManager.LoadScene(0);
    }

    private void Awake() {
        pv = gameObject.GetPhotonView();
    }

    void Start() {
        rejoinText = GlobalValues.Instance.UIElements.gameObject.GetComponentInChildren<RejoinTextUI>();
        if (playerPrefab == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        } else if (PlayerController.LocalPlayerInstance == null) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            
            if (PhotonNetwork.LocalPlayer.ActorNumber < 3) {
                DontDestroyOnLoad(GlobalValues.Instance.gameObject);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                if (PhotonNetwork.IsMasterClient) {
                    GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, GlobalValues.Instance.p1spawn.position, Quaternion.identity, 0);
                    pv.RPC("PlayerSpawnedRPC", RpcTarget.AllBufferedViaServer, true);
                    GlobalValues.Instance.navManager.SetPlayer(false);
                    Camera.main.GetComponent<Animator>().SetTrigger("flyover");
                    player.GetComponent<PlayerInputScript>().StartCameraCutscene(-1);
                } else {
                    GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, GlobalValues.Instance.p2Spawn.position, Quaternion.identity, 0);
                    pv.RPC("PlayerSpawnedRPC", RpcTarget.AllBufferedViaServer, false);
                    GlobalValues.Instance.navManager.SetPlayer(true);
                    Camera.main.GetComponent<Animator>().SetTrigger("flyover");
                    player.GetComponent<PlayerInputScript>().StartCameraCutscene(-1);
                }
            } else {
                pv.RPC("RequestOwnership", RpcTarget.MasterClient);
                Time.timeScale = 0;
                #if !UNITY_EDITOR
                    #if UNITY_WEBGL
                    if(GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceChatEnabled) {
                        setupVoiceChatUnity(PhotonNetwork.CurrentRoom.Name, "client");
                    }
                    #endif
                #endif
                UnPauseGame();
            }
        } else {
            if (PhotonNetwork.IsMasterClient) {
                GlobalValues.Instance.localPlayerInstance.transform.position = GlobalValues.Instance.p1spawn.position;
                pv.RPC("PlayerSpawnedRPC", RpcTarget.AllBufferedViaServer, true);
                GlobalValues.Instance.localPlayerInstance.GetComponent<PlayerInputScript>().StartCameraCutscene(-1);
                Camera.main.GetComponent<Animator>().SetTrigger("flyover");
                if (GlobalValues.Instance.players.Count > 1) {
                    
                }

                GlobalValues.Instance.navManager.SetPlayer(false);
            } else {
                GlobalValues.Instance.localPlayerInstance.GetComponent<PlayerInputScript>().StartCameraCutscene(-1);
                Camera.main.GetComponent<Animator>().SetTrigger("flyover");
                GlobalValues.Instance.localPlayerInstance.transform.position = GlobalValues.Instance.p2Spawn.position;
                pv.RPC("PlayerSpawnedRPC", RpcTarget.AllBufferedViaServer, false);
                GlobalValues.Instance.navManager.SetPlayer(true);
            }
        }
    }

    [PunRPC]
    private void PlayerSpawnedRPC(bool player1) {
        if (player1) {
            GlobalValues.Instance.p1Spawned = true;
        } else {
            GlobalValues.Instance.p2Spawned = true;
        }
        
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player other) {
        otherPlayer = other;
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        if (other.ActorNumber > 2) {
            if (GlobalValues.Instance.localPlayerInstance == GlobalValues.Instance.players[0]) {
                 otherPlayerGO = GlobalValues.Instance.players[1];
            } else {
                otherPlayerGO = GlobalValues.Instance.players[0];
            }
        }
        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
        #if !UNITY_EDITOR
            #if UNITY_WEBGL
            if(GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceChatEnabled) {
                setupVoiceChatUnity(PhotonNetwork.CurrentRoom.Name, "master");
            }
            #endif
        #endif
        UnPauseGame();
    }

    void PauseGame() {
        Time.timeScale = 0;
        rejoinText.SetRejoinText("Waiting for other player to rejoin...");
        rejoinText.DisplayRejoinText(true);
    }

    void UnPauseGame() {
        StartCoroutine(StartCountDown());
    }

    IEnumerator StartCountDown() {
        rejoinText.SetRejoinText("3");
        rejoinText.DisplayRejoinText(true);
        yield return new WaitForSecondsRealtime(1f);
        rejoinText.SetRejoinText("2");
        yield return new WaitForSecondsRealtime(1f);
        rejoinText.SetRejoinText("1");
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
        rejoinText.DisplayRejoinText(false);
    }

    [PunRPC]
    void RequestOwnership() {
        PhotonView otherPlayerView = otherPlayerGO.GetPhotonView();
        otherPlayerView.TransferOwnership(otherPlayer);
        PhotonView[] childPhotons = otherPlayerGO.GetComponentsInChildren<PhotonView>();
        foreach (PhotonView view in childPhotons) {
            view.TransferOwnership(otherPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player other) {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        GlobalValues.Instance.PlayerLeft();
        if (PhotonNetwork.IsMasterClient) {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            PauseGame();
        }
        #if !UNITY_EDITOR
            #if UNITY_WEBGL
                disconnectVoiceChatUnity();
            #endif
        #endif
    }
}