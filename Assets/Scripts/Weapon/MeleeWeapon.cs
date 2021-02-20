using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon {

    public float maxAngle;
    private bool active = false;
    private List<Collider> alreadyHit = new List<Collider>();
    private Vector3 initialAngle;

    public void Awake() {
        initialAngle = transform.parent.localEulerAngles;
    }

    protected override void UseWeapon() {
        active = true;
        Invoke("Deactivate", cooldownTime);
    }

    public void Update() {
        Vector3 a = new Vector3(
            maxAngle * Mathf.Sin((cooldownLeft / cooldownTime) * Mathf.PI),
            0,
            0
        );
        transform.parent.localEulerAngles = initialAngle + a;
    }

    public void Deactivate() {
        active = false;
        alreadyHit.Clear();
        cooldownLeft = 0;
    }

    private void OnTriggerEnter(Collider other) {
        if (active) {
            if (!alreadyHit.Contains(other)) {
                alreadyHit.Add(other);
                Health ds = other.gameObject.GetComponent<Health>();
                if (ds != null) {
                    ds.Damage(damage);
                }
            }
        }
    }
}