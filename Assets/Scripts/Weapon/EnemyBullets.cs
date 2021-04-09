using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.HealthSystem;

namespace LightsOn {
namespace WeaponSystem {
public class EnemyBullets : BulletController {
    LayerMask walls;

    public override void Awake() {
        base.Awake();
        walls = GlobalValues.Instance.environment;
    }

    protected override void OnTriggerEnter(Collider other) {
        if (((1 << other.gameObject.layer) & walls) != 0) {
            if (pv == null || !pv.IsMine) return;
            Health damageScript = other.gameObject.GetComponent<Health>();
            if (damageScript != null)
                damageScript.Damage(damage, hitStunDuration);
            RequestDestroyBullet();
        }
    }
}}}