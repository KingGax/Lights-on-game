using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NavigationPoint : MonoBehaviour
{
    private NavigationManager navigationManager;

    void Start() {
        navigationManager = GlobalValues.Instance.navManager;
    }

    private void OnTriggerEnter(Collider other) {
        if (((1 << other.gameObject.layer) & GlobalValues.Instance.playerOrHiddenPlayerMask) != 0) {
            if (GlobalValues.Instance.localPlayerInstance.transform.position == other.transform.position) {
                navigationManager.UpdateManager(transform.position);
            }
        }
        
    }
}
