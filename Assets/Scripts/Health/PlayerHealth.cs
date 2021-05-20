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

        public override void Start() { //setup values, update healthbar depending on if local player or not
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

        public override void Awake() { //setup bullet layer
            base.Awake();
            bulletLayer = LayerMask.NameToLayer("EnemyBullets");
        }

        public override void Die() { //Kill player and respawn in 2s
            pc.SetMovementEnabled(false);
            pc.Die();
            Invoke("Revive", 2f);
        }

        private void Revive() { //Respawn player, reenable movement and reset health and healthbars
            transform.position = GlobalValues.Instance.fm.GetSpawnPoint();
            health = maxHealth;
            pc.SetMovementEnabled(true);
            hb.UpdateHealth(health);
        }


        public override void Damage(float damage, float stunDuration) { //request damage, update health and healthbars
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
        protected override void DamageRPC(float damage, float stunDuration) { //RPC for requesting damage on player, update healthbars
            base.DamageRPC(damage, stunDuration);
            if (!isLocal) {
                fhb.UpdateHealth(health);
            }
        }

        private void OnTriggerEnter(Collider other) { //On being hit by bullet, feedback hit to bullet and request damage on player
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