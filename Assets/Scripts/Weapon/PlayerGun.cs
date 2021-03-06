using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using LightsOn.AudioSystem;
using LightsOn.HealthSystem;

namespace LightsOn.WeaponSystem {
    public class PlayerGun : PlayerWeapon {
        public Gun primaryFire;
        public MeshRenderer mr;
        public MeshRenderer chargeIndicator;
        public Transform firePoint;
        public GameObject bullet;
        public LineRenderer laser;
        public LineRenderer laserSight;
        public Material chargedSightMat;
        public Material unChargedSightMat;
        public float bulletSpeed;
        public float maxChargeTime;
        public float minChargeTime;
        public float altFiredCooldownTime;
        public float maxLaserDistance;
        public float laserThickness;
        float chargeTime = 0f;
        float laserDist = 0f;
        PhotonView pv;
        AmmoUI ammoScript;


        public void Awake() {
            pv = gameObject.GetPhotonView();
            ammo = maxAmmo;
        }
        protected override void Start() {
            base.Start();
            ammoScript = GlobalValues.Instance.GetComponentInChildren<AmmoUI>();
        }

        public override void UnequipWeapon() {
            mr.enabled = false;
            charging = false;
            chargeTime = 0f;
            HideChargeIndicator();
        }

        //This hides the white ball for when right click is charged
        void HideChargeIndicator() {
            chargeIndicator.enabled = false;
            laserSight.material = unChargedSightMat;
            laserSight.enabled = false;
            laserSight.widthMultiplier = 0.1f;
        }

        //This shows the white ball for when right click is charged
        void ShowChargeIndicator() {
            chargeIndicator.enabled = true;
            laserSight.material = chargedSightMat;
            laserSight.widthMultiplier = 0.2f;
        }

        public override void EquipWeapon() {
            //mr.enabled = true;
            cooldownLeft = equipCooldown;
        }

        protected override void UseWeaponAlt() {
            if (ammo > 0) {
                chargeTime += alternateCooldownTime;
                charging = true;
                if (chargeTime > minChargeTime) {
                    ShowChargeIndicator();
                }
                if (chargeTime > maxChargeTime) {
                    FireAlt();
                }
            }
            else {
                Reload();
            }
            
        }
        private void Update() {
            if (charging) {
                laserSight.enabled = true;
                laserSight.SetPosition(0, firePoint.position);
                laserSight.SetPosition(1, GetHitPoint());
            }
            else {
                laserSight.enabled = false;
            }
        }

        //This is used to find where to draw the laser 
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

        //This is for releasing right click when it is uncharged
        private void FailAlt() {
            chargeTime = 0f;
            charging = false;
            HideChargeIndicator();
        }

        //This is the method that handles firing the laser
        private void FireAlt() {
            chargeTime = 0f;
            charging = false;
            ammo = Mathf.Max(0, ammo - 2);
            cooldownLeft = altFiredCooldownTime;
            pv.RPC("AltFireRPC", RpcTarget.All, firePoint.position, GetHitPoint());
            FireLaser(laserDist);
            HideChargeIndicator();
            if (ammo <= 0) {
                Reload();
            }
        }

        //Syncs the laser visuals across the network
        [PunRPC]
        public void AltFireRPC(Vector3 pos1, Vector3 pos2) {
            laser.SetPosition(0, firePoint.position);
            laser.SetPosition(1, GetHitPoint());
            laser.enabled = true;
            Invoke("DisableLaser", 0.3f);
        }

        //This deals with handling the damage lasers give
        private void FireLaser(float dist) {
            RaycastHit[] hit = Physics.SphereCastAll(firePoint.transform.position, laserThickness, firePoint.transform.up, dist, GlobalValues.Instance.environment | GlobalValues.Instance.shootTargetsLayer).OrderBy(h => h.distance).ToArray();
            foreach (RaycastHit objectHit in hit) {
                EnemyHealth health = objectHit.collider.gameObject.GetComponent<EnemyHealth>();
                if (health != null) {
                    health.Damage(altDamage, altHistunDuration);
                }
            }

            AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXLazer, firePoint.position, gameObject);
        }

        private void DisableLaser() {
            laser.enabled = false;
        }

        public override bool ReleaseWeaponAlt() {
            if (charging) {
                if (chargeTime < minChargeTime) {
                    FailAlt();
                    return false;
                }
                else {
                    FireAlt();
                    return true;
                }
            }
            return false;
        }

        //Gun primary fire
        protected override void UseWeapon() {
            ammo -= 1;
            GameObject newBullet = PhotonNetwork.Instantiate(bullet.name, firePoint.position, transform.rotation);
            BulletController bc = newBullet.GetComponent<BulletController>();
            bc.Fire(damage, hitStunDuration, bulletSpeed, transform.up, 2f);
            AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXShoot, firePoint.position, gameObject);
            if (ammo <= 0) {
                Reload();
            }        
        }
    }
}