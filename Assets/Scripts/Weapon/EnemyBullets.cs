using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.HealthSystem;

namespace LightsOn.WeaponSystem {
    public class EnemyBullets : BulletController {
        LayerMask walls;

        public override void Awake() {
            base.Awake();
            walls = GlobalValues.Instance.environment;
        }

        //This method checks for collisions with walls on the master client, damage is checked locally in player health
        protected override void OnTriggerEnter(Collider other) {
            if (((1 << other.gameObject.layer) & walls) != 0) {
                if (pv == null || !pv.IsMine) return;
                Health damageScript = other.gameObject.GetComponent<Health>();
                if (damageScript != null)
                    damageScript.Damage(damage, hitStunDuration);
                RequestDestroyBullet();
            }
        }
    }
}