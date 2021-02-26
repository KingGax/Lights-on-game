using UnityEngine;
using Photon.Pun;

public sealed class PlayerHealth : Health {

    int bulletLayer;
    HealthBar hb;
    FloatingHealthBar fhb;
    bool isLocal = false;
    public override void Start() {
        base.Start();
        if (gameObject == GlobalValues.Instance.localPlayerInstance){
            isLocal = true;
            Debug.Log("Same object");
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
        
    }

    public override void Damage(float damage)
    {
        //base.Damage(damage);
        pv.RPC("DamageRPC", RpcTarget.All, damage);
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
    protected override void DamageRPC(float damage){
        //Debug.Log("Network damage!!!!!");
        base.DamageRPC(damage);
        if (!isLocal){
            //Debug.Log("Nonlocal");
            fhb.UpdateHealth(health);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == bulletLayer && pv.IsMine) {
            BulletController bc = other.gameObject.GetComponent<BulletController>();
            if (bc != null) {
                Damage(bc.damage);
                bc.RequestDestroyBullet();
            } else {
                Debug.LogError("Something on bullet layer does not have bullet controller");
            }
        }
    }
}