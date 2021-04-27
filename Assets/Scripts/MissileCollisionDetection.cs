using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.HealthSystem;
using Photon.Pun;
public class MissileCollisionDetection : MonoBehaviour
{
    MissileController missile;
    PhotonView pv;
    LayerMask walls;
    // Start is called before the first frame update
    void Start()
    {
        missile = transform.parent.GetComponent<MissileController>();
        pv = transform.parent.GetComponent<PhotonView>();
        walls = GlobalValues.Instance.environment;
    }


    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.layer);
        Debug.Log(((1 << other.gameObject.layer) & walls));
        if (((1 << other.gameObject.layer) & walls) != 0) {
            Debug.Log("go");

            Debug.Log(!pv.IsMine);
            if (pv == null || !pv.IsMine) return;
            Debug.Log("boooom");
            missile.Detonate();
        }
    }


}
