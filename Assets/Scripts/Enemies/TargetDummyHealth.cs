using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetDummyHealth : EnemyHealth
{
    private bool recentlyHit = false;
    // Start is called before the first frame update
    [PunRPC]
    protected override void DamageRPC(float damage, float stunDuration) {
        if (health <= 0) {
            Die();
        }
        recentlyHit = true;
        Invoke("RecentlyHitExpire", 0.3f);
    }
    public void KillTarget() {
        health = 0;
        Damage(0, 0);
    }
    private void RecentlyHitExpire() {
        recentlyHit = false;
    }
    public bool WasHitRecently() {
        return recentlyHit;
    }
}
