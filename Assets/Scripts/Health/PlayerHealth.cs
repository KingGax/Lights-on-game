using UnityEngine;
using Photon.Pun;

public sealed class PlayerHealth : Health {

    int bulletLayer;
    HealthBar hb;
    FloatingHealthBar fhb;
    PlayerController pc;
    
    bool isLocal = false;
    public override void Start() {
        base.Start();
        pc = GetComponent<PlayerController>();
        if (gameObject == GlobalValues.Instance.localPlayerInstance){
            isLocal = true;
            hb = GlobalValues.Instance.UIElements.GetComponentInChildren<HealthBar>();
            hb.SetPlayer();
            hb.UpdateMaxHealth(maxHealth);
            hb.UpdateHealth(health);
        } else {
            isLocal = false;
            fhb = GetComponentInChildren<FloatingHealthBar>();
            fhb.UpdateMaxHealth(maxHealth);
            fhb.UpdateHealth(health);
        }
    }

    public override void Awake() {
        base.Awake();
        bulletLayer = LayerMask.NameToLayer("EnemyBullets");
        
        
    }

    public override void Die() {
        pc.SetMovementEnabled(false);
        Invoke("Revive", 2f);
        /*if (GlobalValues.Instance.players.Count > 1) {
            for (int i = 0; i < GlobalValues.Instance.players.Count; i++) {
                if (GlobalValues.Instance.players[i] != gameObject) {
                    transform.position = GlobalValues.Instance.players[i].transform.position;
                }
            }
        }*/
    }
    private void Revive() {
        transform.position = GlobalValues.Instance.fm.GetSpawnPoint();
        health = maxHealth;
        pc.SetMovementEnabled(true);
        hb.UpdateHealth(health);
    }


    public override void Damage(float damage, float stunDuration)
    {
        //base.Damage(damage);
        pv.RPC("DamageRPC", RpcTarget.All, damage, stunDuration);
        if (isLocal){
            hb.UpdateHealth(health);
        } else {
            fhb.UpdateHealth(health);
        }
        // if (gameObject == GlobalValues.Instance.localPlayerInstance){
        //     //hb = GlobalValues.Instance.UIElements.gameObject.GetComponentInChildren<HealthBar>();
        //     hb.UpdateHealth(health);
        // } else {
        //     fhb.UpdateHealth(health);
        // }
    }

    [PunRPC]
    protected override void DamageRPC(float damage, float stunDuration){
        //Debug.Log("Network damage!!!!!");
        base.DamageRPC(damage, stunDuration);
        if (!isLocal){
            //Debug.Log("Nonlocal");
            fhb.UpdateHealth(health);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == bulletLayer && pv.IsMine) {
            BulletController bc = other.gameObject.GetComponent<BulletController>();
            if (bc != null) {
                Damage(bc.damage, bc.hitStunDuration);
                bc.RequestDestroyBullet();
            } else {
                Debug.LogError("Something on bullet layer does not have bullet controller");
            }
        }
    }
}