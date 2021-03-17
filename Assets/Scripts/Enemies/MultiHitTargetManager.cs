using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiHitTargetManager : MonoBehaviour
{
    TargetDummyHealth[] healths;
    PhotonView pv;
    bool allHit = true;
    bool destroyed = false;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        healths = GetComponentsInChildren<TargetDummyHealth>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!pv.IsMine || pv == null) return;
        allHit = true;
        if (!destroyed) {
            foreach (TargetDummyHealth health in healths) {
                if (!health.WasHitRecently()) {
                    allHit = false;
                    break;
                }
            }
            if (allHit) {
                foreach (TargetDummyHealth health in healths) {
                    health.KillTarget();
                }
                destroyed = true;
                PhotonNetwork.Destroy(gameObject);
            }
        }
        
    }
}
