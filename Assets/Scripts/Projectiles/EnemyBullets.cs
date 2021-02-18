using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullets : BulletController {
    LayerMask walls;

    private void Awake() {
        walls = GlobalValues.Instance.environment;
    }
    private void OnTriggerEnter(Collider other) {
        if (((1 << other.gameObject.layer) & walls) != 0) {
            if (pv == null || !pv.IsMine) return;
            Health damageScript = other.gameObject.GetComponent<Health>();
            if (damageScript != null)
                damageScript.Damage(damage);
            DestroyBullet();
        }
    }
}
