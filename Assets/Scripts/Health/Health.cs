using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Health : MonoBehaviour {

    protected PhotonView pv;
    public float maxHealth;
    private float health;

    public virtual void Awake() {
        pv = GetComponent<PhotonView>();
    }

    void Start() {
        health = maxHealth;
    }

    [PunRPC]
    public void DamageRPC(float damage) {
        if (pv.IsMine) {
            health -= damage;
            if (health < 0) {
                Die();
            }
        }
    }

    public void Damage(float damage) {
        pv.RPC("DamageRPC", RpcTarget.All, damage);
    }

    public virtual void Die() {
        PhotonNetwork.Destroy(gameObject);
    }
}
