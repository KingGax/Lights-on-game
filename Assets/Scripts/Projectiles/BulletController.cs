using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour {
    PhotonView pv;
    float damage;
    float speed;
    Vector3 direction;
    Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        pv = gameObject.GetComponent<PhotonView>();
    }

    [PunRPC]
    void ChildFire(float _damage, float _speed, Vector3 _direction) {
        damage = _damage;
        direction = _direction;
        speed = _speed;
        rb.velocity = direction.normalized * speed;
    }

    public void Fire(float _damage, float _speed, Vector3 _direction) {
        photonView.RPC("ChildFire", RpcTarget.Others, _damage, _speed, _direction);
        damage = _damage;
        direction = _direction;
        speed = _speed;
        rb.velocity = direction.normalized * speed;
        Invoke("DestroyBullet", 2.0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (pv == null || pv.IsMine) return;
        IDamageable damageScript = other.gameObject.GetComponent<IDamageable>();
        if (damageScript != null)
            damageScript.Damage(damage);
        DestroyBullet();
    }

    private void DestroyBullet() {
        PhotonNetwork.Destroy(gameObject);
    }
}
