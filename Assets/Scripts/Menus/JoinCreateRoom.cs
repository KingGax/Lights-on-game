using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.AudioSystem;

public class JoinCreateRoom : MonoBehaviour {
    public GameObject photonConnector;
    void Awake(){
        //photonConnector.SetActive(true);
    }
    public void OnBackBTNPressed() {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("NameMenu");
    }

    public void ConnectToServer(){
        //photonConnector.seta
        // PhotonNetwork.AutomaticallySyncScene = true;
        // PhotonNetwork.ConnectUsingSettings();
    }
}