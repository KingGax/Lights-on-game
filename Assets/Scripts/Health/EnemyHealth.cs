using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealth : Health
{
    // Start is called before the first frame update
    FloatingHealthBar healthBar;
    Enemy controller;
    public override void Start()
    {
        base.Start();
        controller = gameObject.GetComponent<Enemy>();
        healthBar = gameObject.GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateMaxHealth(maxHealth);
        healthBar.UpdateHealth(health);
    }

    public override void Damage(float damage, float stunDuration)
    {
        pv.RPC("DamageRPC", RpcTarget.All, damage, stunDuration);//DamageRPC(damage);
        healthBar.UpdateHealth(health);
    }

    [PunRPC]
    protected override void DamageRPC(float damage, float stunDuration)
    {
        health -= damage;
        if (pv.IsMine){
            controller.RequestHitStun(stunDuration);
            if(health <= 0) {
                
                Die();
            }
        }
        healthBar.UpdateHealth(health);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
