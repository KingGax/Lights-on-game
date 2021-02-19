using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class BulletController : MonoBehaviour {
    public float damage;
    
    protected PhotonView pv;
    protected float speed;
    protected Vector3 direction;
    protected Rigidbody rb;

    public virtual void Awake() {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    protected void ChildFire(double time, float _damage, float _speed, Vector3 _direction) {
        float dt = (float)(PhotonNetwork.Time - time);
        transform.position += direction.normalized * speed * dt;
        damage = _damage;
        direction = _direction;
        speed = _speed;
        rb.velocity = direction.normalized * speed;
    }

    public void Fire(float _damage, float _speed, Vector3 _direction) {
        pv.RPC(
            "ChildFire",
            RpcTarget.Others,
            PhotonNetwork.Time,
            _damage,
            _speed,
            _direction
        );
        damage = _damage;
        direction = _direction;
        speed = _speed;
        rb.velocity = direction.normalized * speed;
        Invoke("RequestDestroyBullet", 2.0f);
    }

    public void RequestDestroyBullet() {
        pv.RPC("DestroyBullet", RpcTarget.All);
    }

    [PunRPC]
    protected void DestroyBullet() {
        if (pv.IsMine) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}