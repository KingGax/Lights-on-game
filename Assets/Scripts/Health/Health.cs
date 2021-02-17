using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Health : MonoBehaviour {

    PhotonView pv;
    public float maxHealth;
    private float health;

    private void Awake() {
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
            Debug.Log("Damage: " + damage);
        }
    }

    public void Damage(float damage) {
        pv.RPC("DamageRPC", RpcTarget.All, damage);
    }

    public virtual void Die() {
        Debug.Log("ded"); 
        PhotonNetwork.Destroy(gameObject);
    }
}
