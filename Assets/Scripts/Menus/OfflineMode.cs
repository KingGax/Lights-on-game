using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using LightsOn.AudioSystem;


public class OfflineMode : MonoBehaviourPunCallbacks {

    public void Start() {
        PhotonNetwork.OfflineMode = true;
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.CreateRoom(
            "Offline",
            new RoomOptions { MaxPlayers = 1 }
        );
    }

    public override void OnJoinedRoom() {
        // AudioManager.Instance.PlayNext();
        PhotonNetwork.LoadLevel("Nightclub");
    }
}