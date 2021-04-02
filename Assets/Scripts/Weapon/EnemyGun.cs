using UnityEngine;
using Photon.Pun;

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
        Debug.Log("Fire!", gameObject);
        bc.Fire(damage, hitStunDuration, bulletSpeed, direction);
    }
}