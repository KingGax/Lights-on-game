using System;
using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    [HideInInspector]
    public float cooldownLeft;
    public float cooldownTime;
    public bool frozen;
    public float damage;
    public bool debug;

    void Start() {
        cooldownLeft = 0;
    }

    // Can be overriden to include bullets left for specific weapons etc
    public virtual bool CanUse() {
        return cooldownLeft <= 0 && !frozen;
    }

    // Returns true if succeeds
    public bool Use() {
        if (!CanUse()) return false;
        cooldownLeft = cooldownTime; // Reset cooldown
        UseWeapon();
        return true;
    }

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
        }
    }
}