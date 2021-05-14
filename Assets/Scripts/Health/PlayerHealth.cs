using UnityEngine;
using Photon.Pun;
using LightsOn.WeaponSystem;
using LightsOn.AudioSystem;

namespace LightsOn.HealthSystem {

    public sealed class PlayerHealth : Health {

        int bulletLayer;
        HealthBar hb;
        public FloatingHealthBar fhb;
        PlayerController pc;

        public Animator animator;

        bool isLocal = false;

        public override void Start() {
            base.Start();
            pc = GetComponent<PlayerController>();
            if (gameObject == GlobalValues.Instance.localPlayerInstance) {
                isLocal = true;
                hb = GlobalValues.Instance.UIElements.GetComponentInChildren<HealthBar>();
                hb.SetPlayer();
                hb.UpdateMaxHealth(maxHealth);
                hb.UpdateHealth(health);
            } else {
                isLocal = false;
                fhb.UpdateMaxHealth(maxHealth);
                fhb.UpdateHealth(health);
            }
        }

        public override void Awake() {
            base.Awake();
            bulletLayer = LayerMask.NameToLayer("EnemyBullets");
        }

        public override void Die() {
            pc.SetMovementEnabled(false);
            pc.Die();
            Invoke("Revive", 2f);
        }

        private void Revive() {
            transform.position = GlobalValues.Instance.fm.GetSpawnPoint();
            health = maxHealth;
            pc.SetMovementEnabled(true);
            hb.UpdateHealth(health);
        }


        public override void Damage(float damage, float stunDuration) {
            pv.RPC("DamageRPC", RpcTarget.All, damage, stunDuration);
            if (isLocal) {
                hb.UpdateHealth(health);
                AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXPlayerHit, transform.position, gameObject);
                animator.SetTrigger("onHit");
            } else {
                fhb.UpdateHealth(health);
            }
        }

        [PunRPC]
        protected override void DamageRPC(float damage, float stunDuration) {
            base.DamageRPC(damage, stunDuration);
            if (!isLocal) {
                fhb.UpdateHealth(health);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == bulletLayer && pv.IsMine) {
                IDamagingProjectile proj = other.gameObject.GetComponent<IDamagingProjectile>();
                if (proj != null) {
                    Damage(proj.GetDamage(), proj.GetHitstun());
                    proj.HitPlayer();
                } else {
                    Debug.LogError("Something on bullet layer does not have IDamagingProjectile");
                } 
            }
        }
    }
}