using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;

namespace LightsOn.WeaponSystem {

    public class EnemyGun : Gun {
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
                    newBullet = PhotonNetwork.Instantiate("Bullets/RedEnemyBullet", firePoint.position, transform.rotation);
                    break;
                case LightColour.Green:
                    newBullet = PhotonNetwork.Instantiate("Bullets/GreenEnemyBullet", firePoint.position, transform.rotation);
                    break;
                case LightColour.Blue:
                    newBullet = PhotonNetwork.Instantiate("Bullets/BlueEnemyBullet", firePoint.position, transform.rotation);
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