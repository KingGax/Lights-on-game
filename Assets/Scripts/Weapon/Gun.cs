using UnityEngine;
using Photon.Pun;

namespace LightsOn {
namespace WeaponSystem {

public class Gun : Weapon {

    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;
    public float bulletTTL = 2f;

    protected override void UseWeapon() {
        GameObject newBullet = PhotonNetwork.Instantiate(bullet.name, firePoint.position, transform.rotation);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, hitStunDuration, bulletSpeed, transform.up, bulletTTL);
    }
}}}