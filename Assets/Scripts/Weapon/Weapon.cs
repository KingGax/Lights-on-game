using System;
using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    [HideInInspector]
    public float cooldownLeft;
    public float primaryCooldownTime;
    public bool frozen;
    public float damage;
    public float hitStunDuration;
    public bool debug;
    protected GameObject target = null;


    protected virtual void Start() {
        cooldownLeft = 0;
    }
    public void SetTarget(int index){
        target = GlobalValues.Instance.players[index];
    }

    public void SetTarget(GameObject g){
        target = g;
    }

    // Can be overriden to include bullets left for specific weapons etc
    public virtual bool CanUse() {
        return cooldownLeft <= 0 && !frozen;
    }

    // Returns true if succeeds
    public bool Use() {
        if (!CanUse()) return false;
        cooldownLeft = primaryCooldownTime; // Reset cooldown
        UseWeapon();
        return true;
    }

    public virtual void UnequipWeapon() { }
    public virtual void EquipWeapon() { }

    public virtual void Freeze() {
        frozen = true;
    }

    public virtual void UnFreeze() {
        frozen = false;
    }

    // Logic for how weapon is used
    protected abstract void UseWeapon();

    public void FixedUpdate() {
        if (!frozen) {
            cooldownLeft -= Time.fixedDeltaTime;
            if (cooldownLeft < 0) {
                cooldownLeft = 0;
            }
        }
    }
}