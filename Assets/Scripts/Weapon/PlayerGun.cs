using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class PlayerGun : PlayerWeapon
{
    public Gun primaryFire;
    public MeshRenderer mr;
    public MeshRenderer chargeIndicator;
    public Transform firePoint;
    public GameObject bullet;
    public LineRenderer laser;
    public float bulletSpeed;
    public float maxChargeTime;
    public float minChargeTime;
    public float altFiredCooldownTime;
    public float maxLaserDistance;
    public float laserThickness;
    float chargeTime = 0f;
    float laserDist = 0f;


    public override void UnequipWeapon() {
        mr.enabled = false;
        charging = false;
        chargeTime = 0f;
        HideChargeIndicator();
    }

    void HideChargeIndicator() {
        chargeIndicator.enabled = false;
    }

    void ShowChargeIndicator() {
        chargeIndicator.enabled = false;
    }

    public override void EquipWeapon() {
        mr.enabled = true;
        cooldownLeft = equipCooldown;
    }

    protected override void UseWeaponAlt() {
        chargeTime += alternateCooldownTime;
        charging = true;
        if (chargeTime > minChargeTime) {
            ShowChargeIndicator();
        }
        if (chargeTime > maxChargeTime) {
            FireAlt();
        }
    }

    private Vector3 GetHitPoint() {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.transform.position, firePoint.transform.up, out hit, maxLaserDistance, GlobalValues.Instance.environment)) {
            laserDist = hit.distance;
            return hit.point;
        }
        else {
            laserDist = maxLaserDistance;
            return firePoint.transform.position + maxLaserDistance * firePoint.transform.up.normalized;
        }
    }

    private void FailAlt() {
        chargeTime = 0f;
        charging = false;
        HideChargeIndicator();
    }

    private void FireAlt() {
        chargeTime = 0f;
        charging = false;
        cooldownLeft = altFiredCooldownTime;
        laser.SetPosition(0, firePoint.position);
        laser.SetPosition(1, GetHitPoint());
        laser.enabled = true;
        FireLaser(laserDist);
        Invoke("DisableLaser", 0.3f);
        HideChargeIndicator();
    }

    private void FireLaser(float dist) {
        RaycastHit[] hit = Physics.SphereCastAll(firePoint.transform.position, laserThickness, firePoint.transform.up, dist, GlobalValues.Instance.environment | GlobalValues.Instance.enemyLayer).OrderBy(h => h.distance).ToArray();
        foreach (RaycastHit objectHit in hit) {
            EnemyHealth health = objectHit.collider.gameObject.GetComponent<EnemyHealth>();
            if (health != null) {
                health.Damage(altDamage, altHistunDuration);
            }
        }
    }

    private void DisableLaser() {
        laser.enabled = false;
    }

    public override void ReleaseWeaponAlt() {
        if (charging) {
            if (chargeTime < minChargeTime) {
                FailAlt();
            }
            else {
                FireAlt();
            }
        }
    }


    protected override void UseWeapon() {
        GameObject newBullet = PhotonNetwork.Instantiate(bullet.name, firePoint.position, transform.rotation);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, hitStunDuration, bulletSpeed, transform.up);
    }
}
