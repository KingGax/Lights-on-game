using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.HealthSystem;

namespace LightsOn.WeaponSystem {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PhotonView))]
    public class BulletController : MonoBehaviour, IDamagingProjectile {
        public float damage;
        public float hitStunDuration;

        protected PhotonView pv;
        protected float speed;
        protected Vector3 direction;
        protected Rigidbody rb;
        protected ParticleSystem[] pSystems;
        protected Collider collider;
        protected Renderer renderer;

        public virtual void Awake() {
            rb = GetComponent<Rigidbody>();
            pv = GetComponent<PhotonView>();
            collider = GetComponent<Collider>();
            renderer = GetComponent<Renderer>();
            pSystems = GetComponentsInChildren<ParticleSystem>();
            Invoke("LocalDestroyBulllet", 10.0f);
        }

        public float GetDamage() {
            return damage;
        }
        public float GetHitstun() {
            return hitStunDuration;
        }
        public void HitPlayer() {
            RequestDestroyBullet();
        }

        //This is the RPC for spawning bullets on the other client
        [PunRPC]
        protected void ChildFire(double time, float _damage, float _hitStunDuration, float _speed, Vector3 _direction) {
            float dt = (float)(PhotonNetwork.Time - time);
            transform.position += direction.normalized * speed * dt;
            damage = _damage;
            hitStunDuration = _hitStunDuration;
            direction = _direction;
            speed = _speed;
            rb.velocity = direction.normalized * speed;
        }

        //This is the local fire method
        public void Fire(float _damage, float _hitStunDuration, float _speed, Vector3 _direction, float ttl) {
            pv.RPC(
                "ChildFire",
                RpcTarget.Others,
                PhotonNetwork.Time,
                _damage,
                _hitStunDuration,
                _speed,
                _direction
            );
            damage = _damage;
            hitStunDuration = _hitStunDuration;
            direction = _direction;
            speed = _speed;
            rb.velocity = direction.normalized * speed;
            Invoke("RequestDestroyBullet", ttl);
        }

        //This allows other clients to request the bullet be destroyed
        public void RequestDestroyBullet() {
            pv.RPC("DestroyBullet", RpcTarget.All);
        }

        private IEnumerator BeforeDeath() {
            yield return new WaitForSeconds(2.0f);
        }

        //This allows other clients to request the bullet be destroyed
        [PunRPC]
        protected IEnumerator DestroyBullet() {
            rb.velocity = Vector3.zero;
            CancelInvoke("RequestDestroyBullet");
            collider.enabled = false;
            renderer.enabled = false;
            if (pSystems.Length > 0) {
                //stop pSystems[0]
                pSystems[0].Stop(true, ParticleSystemStopBehavior.StopEmitting);
                //start all other pSystems

                for (int i = 1; i < pSystems.Length; i++) {
                    pSystems[i].Play(true);
                }
                //destroy after waiting for particles to play
                IEnumerator beforeDeath = BeforeDeath();
                yield return StartCoroutine(beforeDeath);
            }

            if (pv.IsMine) {
                PhotonNetwork.Destroy(gameObject);
            } else {
                LocalDestroyBulllet();
            }
        }

        void LocalDestroyBulllet() {
            if (!pv.IsMine) {
                gameObject.SetActive(false);
            }
        }

        //This checks for collisions with objects on the master client, enemy bullet has a more complex implementation
        protected virtual void OnTriggerEnter(Collider other) {
            if (pv == null || !pv.IsMine) return;
            if (!other.isTrigger) {
                Health damageScript = other.gameObject.GetComponent<Health>();
                if (damageScript != null)
                    damageScript.Damage(damage, hitStunDuration);
                RequestDestroyBullet();
            }
        }
    }
}