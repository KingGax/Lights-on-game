using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;

namespace LightsOn {
namespace WeaponSystem {

public class EnemyGun : Gun {
    
    protected override void UseWeapon() {
        if (target == null){
            SetTarget(0);
        }
        Vector3 direction = target.transform.position - firePoint.position;
       
        GameObject newBullet = PhotonNetwork.Instantiate(bullet.name, firePoint.position, transform.rotation);
        BulletController bc = newBullet.GetComponent<BulletController>();
        LightableObject lo = newBullet.GetComponentInChildren<LightableObject>();
        if (lo != null) {
            lo.SetColour(colour);
        }
        bc.Fire(damage, hitStunDuration, bulletSpeed, direction, bulletTTL);
    }
}}}