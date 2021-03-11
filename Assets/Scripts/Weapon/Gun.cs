using UnityEngine;
using Photon.Pun;

public class Gun : Weapon {

    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;

    protected override void UseWeapon() {
        GameObject newBullet = PhotonNetwork.Instantiate(bullet.name, firePoint.position, transform.rotation);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, hitStunDuration, bulletSpeed, transform.up);
    }
}