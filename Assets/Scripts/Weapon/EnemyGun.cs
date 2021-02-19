using UnityEngine;
using Photon.Pun;

public class EnemyGun : Weapon
{

    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;

    protected override void UseWeapon()
    {
        Vector3 direction = GlobalValues.Instance.players[0].transform.position - firePoint.position;
       
        GameObject newBullet = PhotonNetwork.Instantiate("EnemyBullet", firePoint.position, transform.rotation);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, bulletSpeed, direction);
    }
}