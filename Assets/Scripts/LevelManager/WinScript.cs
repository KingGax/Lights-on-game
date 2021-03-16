using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class WinScript : MonoBehaviour {
    public string sceneName;
    int playerLayer;
    PhotonView pv;
    private void Start() {
        playerLayer = LayerMask.NameToLayer("Player");
        pv = gameObject.GetPhotonView();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == playerLayer) {
            pv.RPC("ChangeSceneRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ChangeSceneRPC() {
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel(sceneName);
        }
    }
}
