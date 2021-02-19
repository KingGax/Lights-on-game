using System;
using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    [HideInInspector]
    public float cooldownLeft;
    public float cooldownTime;
    public float damage;
    public bool debug;

    void Start() {
        cooldownLeft = 0;
        StartCoroutine("CountdownTimers");
    }

    // Can be overriden to include bullets left for specific weapons etc
    public virtual bool CanUse() {
        return cooldownLeft <= 0;
    }

    // Returns true if succeeds
    public bool Use() {
        if (!CanUse()) return false;
        cooldownLeft = cooldownTime; // Reset cooldown
        UseWeapon();
        return true;
    }

    // Logic for how weapon is used
    protected abstract void UseWeapon();

    // TODO consider replacing with FixedUpdate
    private IEnumerator CountdownTimers() {
        while (true) {
            if (cooldownLeft > 0) {
                cooldownLeft -= Time.deltaTime;
            }    
            yield return null;    
        }
    }
}