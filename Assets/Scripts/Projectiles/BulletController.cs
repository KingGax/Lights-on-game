using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour, IPunObservable {
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
        } else {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);
        }
    }
}
