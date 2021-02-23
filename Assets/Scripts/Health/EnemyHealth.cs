using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealth : Health
{
    // Start is called before the first frame update
    FloatingHealthBar healthBar;
    public override void Start()
    {
        base.Start();
        healthBar = gameObject.GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateMaxHealth(maxHealth);
        healthBar.UpdateHealth(health);
    }

    public override void Damage(float damage){
        DamageRPC(damage);
        healthBar.UpdateHealth(health);
    }

    [PunRPC]
    protected override void DamageRPC(float damage)
    {
        base.DamageRPC(damage);
        healthBar.UpdateHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
