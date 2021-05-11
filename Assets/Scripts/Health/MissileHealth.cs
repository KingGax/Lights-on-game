using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace LightsOn.HealthSystem {
    public class MissileHealth : EnemyHealth {
        [PunRPC]
        protected override void DamageRPC(float damage, float stunDuration) {
            health -= damage;
            if (canFlicker) {
                if (flashesRemaining == 0) {
                    flashesRemaining = flashNum;
                }
            }

            if (pv.IsMine) {
                if (health <= 0) {
                    Die();
                }
            }

            healthBar.UpdateHealth(health);
        }
    }
}

