using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.HealthSystem;

namespace LightsOn {
namespace WeaponSystem {

public class DeathBox : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Health ds = other.gameObject.GetComponent<Health>();
        if (ds != null) {
            PhotonView pv = other.gameObject.GetPhotonView();
            if (pv != null && pv.IsMine) {
                ds.Damage(99999, 0);
            }
        }
    }
}}}