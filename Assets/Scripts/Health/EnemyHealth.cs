using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealth : Health
{
    // Start is called before the first frame update
    FloatingHealthBar healthBar;
    Enemy controller;
    Renderer renderer; 
    Material mat;
    Color baseCol;
    Color emisCol;
    int flashNum = 2;
    int flashesRemaining = 0;
    float flashTimerMax = 0.1f;
    float flashTimer;
    bool canFlicker = false;
    public override void Start()
    {
        base.Start();
        controller = gameObject.GetComponent<Enemy>();
        healthBar = gameObject.GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateMaxHealth(maxHealth);
        healthBar.UpdateHealth(health);
        StartCoroutine("Timers");
    }

    public override void Damage(float damage, float stunDuration)
    {
        pv.RPC("DamageRPC", RpcTarget.All, damage, stunDuration);//DamageRPC(damage);
        healthBar.UpdateHealth(health);
    }

    public void InitialiseMaterials(){
        //Debug.Log("Material init");
        if (gameObject.GetComponents<Renderer>().Length == 0){
            renderer = gameObject.GetComponentInChildren<Renderer>();
        } else {
            renderer = gameObject.GetComponent<Renderer>();
        }
        
        mat = renderer.material;
        canFlicker = true;
        baseCol = mat.GetColor("_BaseColor");
        emisCol = mat.GetColor("_EmissionColor");
    }

    [PunRPC]
    protected override void DamageRPC(float damage, float stunDuration)
    {
        health -= damage;
        if (pv.IsMine){
            controller.RequestHitStun(stunDuration);
            if (canFlicker){
                if (flashesRemaining == 0){
                    flashesRemaining = flashNum;
                }
            }
            if(health <= 0) {
                
                Die();
            }
        }
        healthBar.UpdateHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (flashesRemaining > 0 && flashTimer <= 0){
            
            if (flashesRemaining % 2 == 0){
                mat.SetColor("_BaseColor", Color.red);     
                mat.SetColor("_EmissionColour", Color.white);          
            } else {
                mat.SetColor("_BaseColor", baseCol);
                mat.SetColor("_EmissionColour", emisCol);
            }
            flashesRemaining--;
            flashTimer = flashTimerMax;
        }
    }

    private IEnumerator Timers() {
        while (true) {
            if (flashTimer > 0) {
                flashTimer -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
