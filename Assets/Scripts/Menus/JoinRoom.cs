using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _roomCode;
    public Button playButton;

    public void Awake() {
        TextMeshProUGUI t = playButton.GetComponentInChildren<TextMeshProUGUI>();
        t.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
         if (string.IsNullOrEmpty(_roomCode)) {
            playButton.interactable = false;
        }
    }

    public void SetRoomCode(string value) {
        if (!string.IsNullOrEmpty(value)) {
            _roomCode = value;
            PlayerPrefs.SetString(_roomCode,value);
            playButton.interactable = true;
            TextMeshProUGUI t = playButton.GetComponentInChildren<TextMeshProUGUI>();
            t.color = Color.white;
        } else {
            playButton.interactable = false;
            TextMeshProUGUI t = playButton.GetComponentInChildren<TextMeshProUGUI>();
            t.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
    }


    public void OnCLick_JoinButton()
    {
        PhotonNetwork.JoinRoom(_roomCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed: " + message);
    }
}
