using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.LightingSystem;
using Photon.Pun;

namespace LightsOn.WeaponSystem {
    public class BossGun : EnemyGun {
        // Start is called before the first frame update
        protected override void UseWeapon() {
            if (target == null) {
                SetTarget(0);
            }
            Vector3 direction = target.transform.position - firePoint.position;

            GameObject newBullet = null;
            switch (colour) {
                case LightColour.Black:
                    break;
                case LightColour.Red:
                    newBullet = PhotonNetwork.Instantiate("Bullets/RedBossBullet", firePoint.position, transform.rotation);
                    break;
                case LightColour.Green:
                    newBullet = PhotonNetwork.Instantiate("Bullets/GreenBossBullet", firePoint.position, transform.rotation);
                    break;
                case LightColour.Blue:
                    newBullet = PhotonNetwork.Instantiate("Bullets/BlueBossBullet", firePoint.position, transform.rotation);
                    break;
                case LightColour.Cyan:
                    break;
                case LightColour.Magenta:
                    break;
                case LightColour.Yellow:
                    break;
                case LightColour.White:
                    break;
                default:
                    break;
            }
            if (newBullet != null) {
                BulletController bc = newBullet.GetComponent<BulletController>();
                LightableObject lo = newBullet.GetComponentInChildren<LightableObject>();
                bc.Fire(damage, hitStunDuration, bulletSpeed, direction, bulletTTL);
            } else {
                Debug.LogError("Bullet not assigned");
            }

        }
        

}
}

