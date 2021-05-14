using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.AudioSystem;

public class LeaveScript : MonoBehaviour
{
    public void LeaveRoom() {
        Time.timeScale = 1f;
        AudioManager.Instance.playingTrack = 0;
        AudioManager.Instance.nextTrack = 0;
        AudioManager.Instance.PlayNext();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        Destroy(GlobalValues.Instance.UIElements);
        Destroy(GlobalValues.Instance.gameObject);
        SceneManager.LoadScene("StartMenu");
    }
}