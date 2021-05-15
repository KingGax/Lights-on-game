using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.AudioSystem;
using System.Runtime.InteropServices;

public class LeaveScript : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void disconnectVoiceChatUnity();
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
        #if !UNITY_EDITOR
            #if UNITY_WEBGL
                disconnectVoiceChatUnity();
            #endif
        #endif
    }
}