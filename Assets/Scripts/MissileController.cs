using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MissileController : MonoBehaviour
{
    // Start is called before the first frame update
    private float moveSpeed = 0;
    public int framesPerRetarget;
    public int damage;
    public float hitstun;
    private float turnSpeed;
    private int retargetCounter;
    private float missileDeathDelay = 5f;
    private float explosionRadius = 1.3f;
    public GameObject deathEffect;
    GameObject targetPlayer;
    PhotonView pv;
    bool started = false;
    bool fireOnStart = false;
    Collider missileCollider;
    Collider childCollider;
    SkinnedMeshRenderer renderer;
    HealthBar hb;
    public void Fire(float speed,float _turnSpeed, int _damage) {
        moveSpeed = speed;
        turnSpeed = _turnSpeed;
        damage = _damage;
        if (started) {
            pv.RPC("FireRPC", RpcTarget.All, speed, _turnSpeed,_damage);
        } else {
            fireOnStart = true;
        }
        
    }
    [PunRPC]
    private void FireRPC(float speed, float _turnSpeed, int _damage) {
        moveSpeed = speed;
        turnSpeed = _turnSpeed;
        damage = _damage;
    }
    public void RequestDestroy() {
        pv.RPC("RequestDestroyRPC", RpcTarget.All);
    }

    [PunRPC]
    private void RequestDestroyRPC() {
        if (pv.IsMine) {
            Detonate();
        }
    }


    void Start()
    {
        retargetCounter = framesPerRetarget;
        pv = GetComponent<PhotonView>();
        SelectTarget();
        if (fireOnStart) {
            pv.RPC("FireRPC", RpcTarget.All, moveSpeed, turnSpeed, damage);
        }
        transform.up = targetPlayer.transform.position - transform.position;
        missileCollider = GetComponent<Collider>();
        childCollider = GetComponentInChildren<Collider>();
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        hb = GetComponentInChildren<HealthBar>();
    }

    private void SpawnDeathEffect(Vector3 explosionPoint) {
        Instantiate(deathEffect, explosionPoint, Quaternion.identity);
    }

    [PunRPC]
    private void DetonateRPC(Vector3 deathPos) {
        DisableMissile();
        if (pv == null || !pv.IsMine) {
            SpawnDeathEffect(deathPos);
        } else if (pv.IsMine) {
            SpawnDeathEffect(deathPos);
            Invoke("DespawnMissile", 2f);
        }
        
    }

    private void DisableMissile() {
        childCollider.enabled = false;
        missileCollider.enabled = false;
        renderer.enabled = false;
        moveSpeed = 0;
        hb.gameObject.SetActive(false);
        
    }

    private void DespawnMissile() {
        PhotonNetwork.Destroy(gameObject);
    }


    public void Detonate() {
        pv.RPC("DetonateRPC", RpcTarget.All, transform.position);
    }

    protected int SelectTarget() { //default implementation sets target as closest player
        float minDist = Mathf.Infinity;
        int targetIndex = 0;
        for (int i = 0; i < GlobalValues.Instance.players.Count; i++) {
            float distToPlayer = Vector3.Distance(gameObject.transform.position, GlobalValues.Instance.players[i].transform.position);
            if (distToPlayer < minDist) {
                //Debug.Log("Player distance: "+ distToPlayer);
                minDist = distToPlayer;
                targetIndex = i;
            }
        }
        targetPlayer = GlobalValues.Instance.players[targetIndex];
        return targetIndex;
    }


    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if (retargetCounter == 0) {
            retargetCounter = framesPerRetarget;
            SelectTarget();
        }
        Vector3 targetPostition = new Vector3(targetPlayer.transform.position.x,
                                        transform.position.y,
                                        targetPlayer.transform.position.z);
        transform.up = Vector3.RotateTowards(transform.up, targetPostition - transform.position,Time.deltaTime*turnSpeed,0);
        transform.position += transform.up * moveSpeed * Time.deltaTime + new Vector3(0,(targetPlayer.transform.position.y+targetPlayer.transform.localScale.y - transform.position.y)*Time.deltaTime,0);
        retargetCounter--;   
    }
}
