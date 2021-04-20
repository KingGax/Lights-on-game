using UnityEngine;
using Photon.Pun;
using LightsOn.AudioSystem;

namespace LightsOn {
namespace HealthSystem {

[RequireComponent(typeof(PhotonView))]
public class Health : MonoBehaviour {

    [MinAttribute(1)]
    public float maxHealth;
    protected float health;
    protected PhotonView pv;

    public virtual void Awake() {
        pv = gameObject.GetComponent<PhotonView>();
    }

    public virtual void Start() {
        health = maxHealth;
    }

    [PunRPC]
    protected virtual void DamageRPC(float damage, float stunDuration) {
        if (health > 0) {
            health -= damage;
            if (pv.IsMine && health <= 0) {
                Die();
            }
        }
    }

    public virtual void Damage(float damage, float stunDuration) {
        pv.RPC("DamageRPC", RpcTarget.All, damage, stunDuration);
    }

    public virtual void DeathEffects() {

    }

    [PunRPC]
    public void DieRPC() {
        if (pv.IsMine) {
            PhotonNetwork.CleanRpcBufferIfMine(pv);
        }
        Destroy(gameObject);
    }

    public virtual void Die() {
        AudioManager.PlaySFX(SoundClips.Instance.SFXKill, transform.position);
        pv.RPC("DieRPC", RpcTarget.AllBuffered);
    }

    public float getHealth() {
        return health;
    }
}}}