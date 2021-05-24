using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NavigationPoint : MonoBehaviour //Class for navigation compass points
{
    private NavigationManager navigationManager;

    void Start() {
        navigationManager = GlobalValues.Instance.navManager;
    }

    private void OnTriggerEnter(Collider other) { //Oncollision with player, tell manager to update next navpoint
        if (((1 << other.gameObject.layer) & GlobalValues.Instance.playerOrHiddenPlayerMask) != 0){
            //PhotonView pv = other.gameObject.GetComponent<PhotonView>();
            if (GlobalValues.Instance.localPlayerInstance.transform.position != null) {
                if (GlobalValues.Instance.localPlayerInstance.transform.position == other.transform.position) {
                    navigationManager.UpdateManager(transform.position);
                }
            }
        }
    }
}
