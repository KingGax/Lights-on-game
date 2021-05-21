using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using LightsOn.LightingSystem;
using LightsOn.AudioSystem;

public class WinScript : MonoBehaviour {

    public string sceneName;
    LayerMask playerLayers;
    bool loadingLevel = false;
    PhotonView pv;

    private void Start() {
        playerLayers = GlobalValues.Instance.playerOrHiddenPlayerMask;
        pv = gameObject.GetPhotonView();
    }

    private void OnTriggerEnter(Collider other) {
        if (((1<<other.gameObject.layer) | playerLayers) != 0) {
            pv.RPC("ChangeSceneRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public IEnumerator ChangeSceneRPC() {
        PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
        if (!loadingLevel && GlobalValues.Instance.fm.GetObjectivesTriggered()) {
            AudioManager.Instance.PlayNext();
        }

        if (PhotonNetwork.IsMasterClient && !loadingLevel && GlobalValues.Instance.fm.GetObjectivesTriggered()) {
            loadingLevel = true;
            yield return new WaitForSeconds(1);
            GlobalValues.Instance.localPlayerInstance.GetComponentInChildren<Lanturn>().BufferLightColour();
            PhotonNetwork.LoadLevel(sceneName);
        }
    }
}
