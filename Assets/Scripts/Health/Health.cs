using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Health : MonoBehaviour
{

    protected PhotonView pv;
    public float maxHealth;
    protected float health;

    public virtual void Awake() {
        pv = gameObject.GetComponent<PhotonView>();
    }

    public virtual void Start() {
        health = maxHealth;
    }

    [PunRPC]
    protected virtual void DamageRPC(float damage) {
        if (health > 0) {
            health -= damage;
            if (pv.IsMine && health <= 0) {
                Die();
            }
        }
    }

    public virtual void Damage(float damage)
    {
        pv.RPC("DamageRPC", RpcTarget.All, damage);
    }

    public virtual void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
