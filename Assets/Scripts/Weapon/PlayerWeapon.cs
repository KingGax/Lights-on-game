using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : Weapon
{
    public float alternateCooldownTime;
    protected bool charging = false;
    public float chargeSpeedModifier = 1f;
    public float altDamage;
    public float altHistunDuration;
    protected override abstract void UseWeapon();
    protected virtual void UseWeaponAlt() { }

    public virtual void ReleaseWeaponAlt() { }

    public bool IsCharging() {
        return charging;
    }
    public bool UseAlt() {
        if (!CanUse()) return false;
        cooldownLeft = alternateCooldownTime; // Reset cooldown
        UseWeaponAlt();
        return true;
    }

}
