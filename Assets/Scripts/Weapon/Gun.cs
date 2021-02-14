using UnityEngine;

public class Gun : Weapon {

    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;

    protected override void UseWeapon() {
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, bulletSpeed, transform.up);
        newBullet.transform.up = transform.up;
        Destroy(newBullet, 2.0f);
    }
}