using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseMeleeWeapon : MonoBehaviour
{
    float damage = 0;
    bool active = false;
    List<Collider> alreadyHit = new List<Collider>();
    public virtual void Swing(float _damage, float activityTime) {
        damage = _damage;
        active = true;
        Invoke("Deactivate", activityTime);
    }
    
    void Deactivate() {
        active = false;
        alreadyHit.Clear();
    }

    private void OnTriggerEnter(Collider other) {
        if (active) {
            if (!alreadyHit.Contains(other)) {
                alreadyHit.Add(other);
                IDamageable ds = other.gameObject.GetComponent<IDamageable>();
                if (ds != null) {
                    ds.Damage(damage);
                }
            }
        }
    }
}
