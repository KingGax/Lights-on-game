using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactWeapon : MonoBehaviour {
    public float damage;
    public float knockback;
    public float knockbackDuration;

    bool active = false;
    ChargerEnemyController parentScript;
    List<Collider> alreadyHit = new List<Collider>();

    void Awake() {
        damage = 0f;
        knockback = 0f;
        knockbackDuration = 0f;
        parentScript = gameObject.GetComponentInParent<ChargerEnemyController>();
    }

    public void Activate() {
        active = true;
        alreadyHit.Clear();
    }
    public void Deactivate() {
        active = false;
        alreadyHit.Clear();
    }

    private void OnTriggerEnter(Collider other) {
        if (active) {
            if (!alreadyHit.Contains(other)) {
                alreadyHit.Add(other);
                Health ds = other.gameObject.GetComponent<Health>();
                if (ds != null) {
                    ds.Damage(damage);
                    IKnockbackable ks = other.gameObject.GetComponent<IKnockbackable>();
                    if (ks != null) {
                        Vector3 dir = gameObject.transform.forward; // this might need to be changed
                        ks.TakeKnockback(dir, knockback, knockbackDuration);
                    }
                    parentScript.ChangeToChargeEnd();
                }
            }
        }
    }
}
