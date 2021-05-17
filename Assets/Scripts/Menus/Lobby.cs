using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.InteropServices;
using TMPro;
using LightsOn.AudioSystem;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject startButton;

    [SerializeField]
    private GameObject gameMode;
    [DllImport("__Internal")]
    private static extern void setupVoiceChatUnity(string roomName, string role);

    [SerializeField]
    private GameObject roomCode;
    public GameObject listingsPrefab;
    public Button readyBtn;

    private bool loadingScene = false;
    public TransitionTrigger transition;

    private string nextScene;

    void Awake()
    {
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0.1f;
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
            gameMode.SetActive(true);
            nextScene = "Tutorial";
            Vector3 newLoc = transform.position + new Vector3(-10, 0, 0);
            GameObject listings = PhotonNetwork.Instantiate(listingsPrefab.name, newLoc, new Quaternion(0, 0, 0, 0), 0);
            PlayerListingsMenu listingsMenu = listings.GetComponent<PlayerListingsMenu>();
            readyBtn.GetComponent<Button>().onClick.AddListener(listingsMenu.ToggleReady);
            ////readyBtn.
            //listings.GetComponent<Canvas>.SIZE
            //PlayerListingsMenu lmenu = listings.GetComponent<PlayerListingsMenu>();
            listings.transform.SetParent(transform);
            #if !UNITY_EDITOR
                #if UNITY_WEBGL
                if(GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceChatEnabled) {
                    setupVoiceChatUnity(PhotonNetwork.CurrentRoom.Name, "master");
                }
                #endif
            #endif
        } else {
            #if !UNITY_EDITOR
                #if UNITY_WEBGL
                if(GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceChatEnabled) {
                    setupVoiceChatUnity(PhotonNetwork.CurrentRoom.Name, "client");
                }
                #endif
            #endif
        }
        TextMeshProUGUI t = roomCode.GetComponentInChildren<TextMeshProUGUI>();
        t.text = PhotonNetwork.CurrentRoom.Name;
        loadingScene = false;
        
    }

    public void SetReadyButton() {
        readyBtn.onClick.AddListener(GetComponentInChildren<PlayerListingsMenu>().ToggleReady);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient) {
            gameMode.SetActive(true);
            nextScene = "Tutorial";
            startButton.SetActive(true);
        }
    }

    public void StartGame() {
        if(loadingScene == false){
            PlayerListingsMenu listingsMenu = GetComponentInChildren<PlayerListingsMenu>();
            if (listingsMenu.isReady() && PhotonNetwork.PlayerList.Length == 2){
                loadingScene = true;
                transition.mouseClick();
                //PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
                PhotonNetwork.LoadLevel(nextScene);
                //Initiated voice chat here
            } else {
            }
        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();

        //PhotonNetwork.Reconnect();
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene("JoinRoomMenu");
    }

    public void CopyRoomCodeToClipboard() {
        TextMeshProUGUI t = roomCode.GetComponentInChildren<TextMeshProUGUI>();
        GUIUtility.systemCopyBuffer = t.text;
    }

    public void HandleGameModeChange(int index){
        switch(index){
            case 0:
                nextScene = "Tutorial";
                break;
            case 1:
                nextScene = "Endless_waves_1";
                break;
            default:
                return;
        }
    }
}
