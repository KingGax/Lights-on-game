using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MeleeWeapon : Weapon {

    public float maxAngle;
    private List<Collider> alreadyHit = new List<Collider>();
    private Vector3 initialAngle;
    private PhotonView weaponPhotonView;

    public void Awake() {
        initialAngle = transform.parent.localEulerAngles;
        weaponPhotonView = gameObject.GetPhotonView();
    }

    [PunRPC]
    protected void RPCUseWeapon(double time) {
        alreadyHit.Clear();
        float dt = (float)(PhotonNetwork.Time - time);
        cooldownLeft = cooldownTime - dt;
    }

    protected override void UseWeapon() { 
        weaponPhotonView.RPC("RPCUseWeapon", RpcTarget.All, PhotonNetwork.Time);
    }

    public void Update() {
        if (!CanUse()) {
            Vector3 a = new Vector3(
                maxAngle * Mathf.Sin((cooldownLeft / cooldownTime) * Mathf.PI),
                0,
                0
            );
            transform.parent.localEulerAngles = initialAngle + a;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == GlobalValues.Instance.localPlayerInstance)
        {
            if (!CanUse() && !frozen && !alreadyHit.Contains(other))
            {
                alreadyHit.Add(other);
                Health ds = other.gameObject.GetComponent<Health>();
                if (ds != null)
                {
                    ds.Damage(damage, hitStunDuration);
                }
            }
        }
    }
}