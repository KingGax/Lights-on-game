using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullets : BulletController {

    private void OnTriggerEnter(Collider other) {
        Debug.Log("trigger");
        if (pv == null || !pv.IsMine) return;
        Debug.Log("death");
        Health damageScript = other.gameObject.GetComponent<Health>();
        if (damageScript != null)
            damageScript.Damage(damage);
        RequestDestroyBullet();
    }
}
