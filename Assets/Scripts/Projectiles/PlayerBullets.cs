using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullets : BulletController {

    private void OnTriggerEnter(Collider other) {
        if (pv == null || !pv.IsMine) return;
        Health damageScript = other.gameObject.GetComponent<Health>();
        if (damageScript != null)
            damageScript.Damage(damage, hitStunDuration);
        RequestDestroyBullet();
    }
}
