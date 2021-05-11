using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightsOn {
namespace WeaponSystem {

public abstract class PlayerWeapon : Weapon {
    public float alternateCooldownTime;
    protected bool charging = false;
    public float chargeSpeedModifier = 1f;
    public float altDamage;
    public float altHistunDuration;
    public float equipCooldown;
    public int maxAmmo;
    public float reloadTime;
    protected int ammo;
    private bool reloading;
    protected override abstract void UseWeapon();
    protected virtual void UseWeaponAlt() { }

    public virtual void ReleaseWeaponAlt() { }

    public virtual void Reload() {
        if (ammo < maxAmmo) {
            Invoke("FillAmmo", reloadTime);
            reloading = true;
        }
    }

    private void FillAmmo() {
        ammo = maxAmmo;
        reloading = false;
    }
    public void InterruptReload() {
        reloading = false;
        CancelInvoke("FillAmmo");
    }

    public int GetAmmo() {
        return ammo;
    }


    public bool IsReloading() {
        return reloading;
    }

    public override bool CanUse() {
        return base.CanUse() && !reloading;
    }

    public bool IsCharging() {
        return charging;
    }
    public bool UseAlt() {
        if (!CanUse()) return false;
        cooldownLeft = alternateCooldownTime; // Reset cooldown
        UseWeaponAlt();
        return true;
    }

}}}