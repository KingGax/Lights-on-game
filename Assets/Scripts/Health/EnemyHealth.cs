using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;
using LightsOn.AudioSystem;


namespace LightsOn.HealthSystem {
    public class EnemyHealth : Health {
        protected HealthBar healthBar;
        Enemy controller;
        Renderer renderer;
        LightableEnemy lightableEnemy;
        Material mat;
        Color baseCol;
        Color emisCol;
        bool hasStarted = false;
        protected int flashNum = 2;
        protected int flashesRemaining = 0;
        protected float flashTimerMax = 0.1f;
        protected float flashTimer;
        protected bool canFlicker = false;

        public override void Start() { //Get components, start coroutine and update health/healthbar
            base.Start();
            controller = gameObject.GetComponent<Enemy>(); 
            healthBar = gameObject.GetComponentInChildren<HealthBar>();
            healthBar.UpdateMaxHealth(maxHealth); //calls this after setting boss HB parent [FIX THIS]
            healthBar.UpdateHealth(health);
            lightableEnemy = GetComponentInChildren<LightableEnemy>();
            StartCoroutine("Timers");
            hasStarted = true;
        }

        public override void Damage(float damage, float stunDuration) { //Request damage on enemyl, update health bar
            pv.RPC("DamageRPC", RpcTarget.All, damage, stunDuration);
            healthBar.UpdateHealth(health);
        }

        public void InitialiseMaterials() { //Gets materials for enemy flashing on receiving damage
            if (!hasStarted){
                Start();
            }
            if (lightableEnemy.usesMeshRenderer){
                if (gameObject.GetComponents<MeshRenderer>().Length == 0) {
                    renderer = gameObject.GetComponentInChildren<MeshRenderer>();
                } else {
                    renderer = gameObject.GetComponent<MeshRenderer>();
                }
            } else {
                if (gameObject.GetComponents<SkinnedMeshRenderer>().Length == 0) {
                    renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                } else {
                    renderer = gameObject.GetComponent<SkinnedMeshRenderer>();
                }
            }
            mat = renderer.material;
            canFlicker = true;
            baseCol = mat.GetColor("_BaseColor");
            emisCol = mat.GetColor("_EmissionColor");
        }

        [PunRPC]
        protected override void DamageRPC(float damage, float stunDuration) { //RPC for requesting damage on enemy, starting enemy flashing
            health -= damage;
            if (canFlicker) {
                if (flashesRemaining == 0) {
                    flashesRemaining = flashNum;
                }
            }
            if (pv.IsMine) {
                controller.RequestHitStun(stunDuration);
                if (health <= 0) {
                    pv.RPC("SpawnDeathBoids", RpcTarget.All);
                    Die();
                }
            }
            healthBar.UpdateHealth(health);
        }

        [PunRPC]
        void SpawnDeathBoids() { //Spawn swarms for dead enemy
            LightableEnemy enemyLightable = GetComponentInChildren<LightableEnemy>();
            if (enemyLightable != null) {
                enemyLightable.SpawnDeathCloud();
            }
        }

        public override void Die() //Death handler - call SFX and RPC on all clients
        {
            AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXKill, transform.position, gameObject);
            pv.RPC("EnemyDieRPC", RpcTarget.AllBuffered);
        }
        
        [PunRPC]
        public void EnemyDieRPC() { //RPC for enemy death
            if (pv.IsMine) {
                PhotonNetwork.CleanRpcBufferIfMine(pv);
            }
            Destroy(gameObject);
            Destroy(healthBar.gameObject);
        }

        void Update() { //handles enemy flashing logic
            if (flashesRemaining > 0 && flashTimer <= 0) {
                if (flashesRemaining % 2 == 0) {
                    if (lightableEnemy.childObjects != null) {
                        foreach (Renderer r in lightableEnemy.childObjects) {
                            r.material.SetColor("_BaseColor", Color.red);
                            r.material.SetColor("_EmissionColour", Color.white);
                        }
                    }
                    mat.SetColor("_BaseColor", Color.red);
                    mat.SetColor("_EmissionColour", Color.white);
                } else {
                     if (lightableEnemy.childObjects != null) {
                        foreach (Renderer r in lightableEnemy.childObjects) {
                            r.material.SetColor("_BaseColor", baseCol);
                            r.material.SetColor("_EmissionColour", emisCol);
                        }
                    }
                    mat.SetColor("_BaseColor", baseCol);
                    mat.SetColor("_EmissionColour", emisCol);
                }
                flashesRemaining--;
                flashTimer = flashTimerMax;
            }
        }

        private IEnumerator Timers() { //coroutine for timers
            while (true) {
                if (flashTimer > 0) {
                    flashTimer -= Time.deltaTime;
                }

                yield return null;
            }
        }
    }
}