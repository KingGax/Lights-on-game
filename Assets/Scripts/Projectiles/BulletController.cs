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

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    void ChildFire(float time, float _damage, float _speed, Vector3 _direction) {
        damage = _damage;
        direction = _direction;
        speed = _speed;
        rb.velocity = direction.normalized * speed;
        transform.position += direction.normalized * speed * (float)(PhotonNetwork.Time - time);
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
        Invoke("DestroyBullet", 2.0f);
    }

    public void DestroyBullet() {
        PhotonNetwork.Destroy(gameObject);
    }
}
