using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.AudioSystem;

public class JoinCreateRoom : MonoBehaviour {

    public void OnBackBTNPressed() {
        PhotonNetwork.LoadLevel("NameMenu");
    }
}