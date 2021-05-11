using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using LightsOn.HealthSystem;

namespace LightsOn {
namespace WeaponSystem {

public class PlayerSword : PlayerWeapon {

    public float maxAngle;
    private List<Collider> alreadyHit = new List<Collider>();
    private Vector3 initialAngle;
    private PhotonView weaponPhotonView;
    public MeshRenderer mr;
    private bool active = false;

    public void Awake() {
        initialAngle = transform.parent.localEulerAngles;
        weaponPhotonView = gameObject.GetPhotonView();
    }

    [PunRPC]
    protected void RPCUseWeapon(double time) {
        alreadyHit.Clear();
        active = true;
        float dt = (float)(PhotonNetwork.Time - time);
        cooldownLeft = primaryCooldownTime - dt;
    }

    public override void UnequipWeapon() {
        mr.enabled = false;
        cooldownLeft = 0f;
        active = false;
    }
    public override void EquipWeapon() {
        mr.enabled = true;
        cooldownLeft = equipCooldown;
    }

    protected override void UseWeapon() {
        weaponPhotonView.RPC("RPCUseWeapon", RpcTarget.All, PhotonNetwork.Time);
    }

    public void Update() {
        if (!CanUse()) {
            Vector3 a = new Vector3(
                maxAngle * Mathf.Sin((cooldownLeft / primaryCooldownTime) * Mathf.PI),
                0,
                0
            );
            transform.parent.localEulerAngles = initialAngle + a;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!CanUse() && !frozen && !alreadyHit.Contains(other) && active) {
            alreadyHit.Add(other);
            Health ds = other.gameObject.GetComponent<Health>();
            if (ds != null) {
                ds.Damage(damage, hitStunDuration);
            }
        }
    }
}}}