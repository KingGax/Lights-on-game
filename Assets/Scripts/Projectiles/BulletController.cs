using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    float damage;
    float speed;
    Vector3 direction;
    Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    public void Fire(float _damage, float _speed, Vector3 _direction) {
        damage = _damage;
        direction = _direction;
        speed = _speed;
        rb.velocity = direction.normalized * speed;
    }

    private void OnTriggerEnter(Collider other) {
        IDamageable damageScript = other.gameObject.GetComponent<IDamageable>();
        if (damageScript != null)
            damageScript.Damage(damage);
        DestroyBullet();
    }

    void DestroyBullet() {
        Destroy(gameObject);
    }
}
