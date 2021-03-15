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

    private bool loadingScene = false;

    void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
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
            loadingScene = true;
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
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
        Debug.Log("coppied to clipboard: " + t.text);
    }
}
