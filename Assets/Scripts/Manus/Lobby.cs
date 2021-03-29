using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject roomCode;
    public GameObject listingsPrefab;

    private bool loadingScene = false;

    void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
            Vector3 newLoc = transform.position + new Vector3(-10, 0, 0);
            GameObject listings = PhotonNetwork.Instantiate(listingsPrefab.name, newLoc, new Quaternion(0, 0, 0, 0), 0);
            //PlayerListingsMenu lmenu = listings.GetComponent<PlayerListingsMenu>();
            listings.transform.SetParent(transform);
        } 
        TextMeshProUGUI t = roomCode.GetComponentInChildren<TextMeshProUGUI>();
        t.text = PhotonNetwork.CurrentRoom.Name;
        loadingScene = false;
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient) {
            startButton.SetActive(true);
        }
    }

    public void StartGame() {
        if(loadingScene == false){
            PlayerListingsMenu listingsMenu = GetComponentInChildren<PlayerListingsMenu>();
            if (listingsMenu.isReady()){
                loadingScene = true;
                PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
            } else {
                Debug.Log("Please ensure everyone is 'Ready' before starting the game.");
            }
        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene(0);
    }

    public void CopyRoomCodeToClipboard() {
        TextMeshProUGUI t = roomCode.GetComponentInChildren<TextMeshProUGUI>();
        GUIUtility.systemCopyBuffer = t.text;
        Debug.Log("copied to clipboard: " + t.text);
    }
}
