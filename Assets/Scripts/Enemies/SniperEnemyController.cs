using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SniperEnemyController : Enemy
{
    EnemyState enemyState;
    public float detectionThreshold;
    public float shootPrepareTimerMax;
    float shootPrepareTimer;
    public float shootRecoverTimerMax;
    float shootRecoverTimer;
    public float shotFlashDuration;
    float losCheckTimer;
    public float losCheckTimerMax;
    public GameObject bullet;
    LightableColour bulletColour;
    float pathStoppingThreshold = 0.01f;
    public float maxLaserDistance;
    LineRenderer laser;
    
    enum EnemyState {
        Shooting, //Actively attacking the player
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        ShootPrepare, //Preparing to shoot
        ShootRecover, //Recovering from shot/reloading
        GettingLOS //Trying to find LOS
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        laser = GetComponentInChildren<LineRenderer>();
        //hitStunned = false;
        inStunnableState = true;
    }

    public override void Awake()
    {
        base.Awake();
        if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
            hasPlayerJoined = true;
            int index = SelectTarget();
            weapon.SetTarget(index);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if (!hasPlayerJoined){
            if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
                hasPlayerJoined = true;
                int index = SelectTarget();
                weapon.SetTarget(index);
            } else {
                return;
            }
        } 
        //playerObj = GlobalValues.Instance.players[0];
        if (aiEnabled) {
            switch (enemyState) {
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.ShootPrepare:
                    ShootPrepare();
                    break;
                case EnemyState.Shooting:
                    Shooting();
                    break;
                case EnemyState.ShootRecover:
                    ShootRecover();
                    break;
                case EnemyState.GettingLOS:
                    GettingLOS();
                    break;
                default:
                    break;
            }
        }
    }

    Vector3 GetLaserPosition(){
        RaycastHit hit;
        Vector3 playerDirection = GetPlayerDirection();
        if (Physics.Raycast(transform.position, playerDirection, out hit, maxLaserDistance, GlobalValues.Instance.environment | GlobalValues.Instance.playerLayer)){
            return hit.point;
        } else{
            return transform.position;
        }
    }

    Vector3 GetPlayerDirection(){
        Vector3 playerDirection = playerObj.transform.position - transform.position;
        playerDirection.y = 0f;
        return playerDirection;
    }

    void Patrol() {
        float minDist  = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < GlobalValues.Instance.players.Count; i++){
            float distToPlayer = Vector3.Distance(GlobalValues.Instance.players[i].transform.position, transform.position);
            if (distToPlayer < minDist){
                minDist = distToPlayer;
                index = i;
            }
        }
        playerObj = GlobalValues.Instance.players[index];
        if (minDist < detectionThreshold)
        {
            ChangeToGettingLOS();
        }
    }

    void ChangeToShootPrepare(){
        enemyState = EnemyState.ShootPrepare;
        
        //activate laser
        laser.enabled = true;
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, GetLaserPosition());
        shootPrepareTimer = shootPrepareTimerMax;
        Debug.Log("Preparing shot");
    }

    void ShootPrepare(){
        Vector3 playerDirection = GetPlayerDirection();
        TurnTowards(playerDirection); //not working
        laser.SetPosition(1, GetLaserPosition());
        if (shootPrepareTimer <= shotFlashDuration){
            inStunnableState = false; //enemy can't be hitstunned while about to shoot
            //flash
        }
        if (shootPrepareTimer <= 0){
            Debug.Log("Sniper shooting");
            ChangeToShooting();
        }
    }

    void ChangeToShooting(){
        //if (HasPlayerLOS(playerObj, detectionThreshold)) {
        agent.enabled = false;
        enemyState = EnemyState.Shooting;
        laser.enabled = false;
        // } else {
        //     ChangeToGettingLOS();
        // }
    }

    void Shooting(){
        Vector3 playerDirection = playerObj.transform.position - transform.position;
        playerDirection.y = 0f;
        TurnTowards(playerDirection);
        weapon.Use();
        ChangeToShootRecover();
    }

    void ChangeToShootRecover(){
        enemyState = EnemyState.ShootRecover;
        shootRecoverTimer = shootRecoverTimerMax;
        inStunnableState = true;
        agent.enabled = true;
        Debug.Log("Reloading weapon");
    }

    void ShootRecover(){
        //play reload animation?
        if (shootRecoverTimer <= 0){
            ChangeToGettingLOS();
        }
    }

    void ChangeToGettingLOS(){
        int index = SelectTarget();
        weapon.SetTarget(index);
        losCheckTimer = losCheckTimerMax;
        agent.enabled = true;
        agent.destination = playerObj.transform.position;
        enemyState = EnemyState.GettingLOS;
    }

    void GettingLOS(){
        if (losCheckTimer <= 0) {
            //Debug.Log("Checking LOS again!");
            if (HasPlayerLOS(playerObj, detectionThreshold)) {
                agent.enabled = false;
                ChangeToShootPrepare();
            } else {
                //check if stuck
                if (agent.remainingDistance != Mathf.Infinity && agent.remainingDistance <= pathStoppingThreshold) {
                    //path complete. credit: https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
                    ChangeToGettingLOS();
                }
                losCheckTimer = losCheckTimerMax;
            }
        }
    }

    private IEnumerator EnemyTimers() {
        while (true) {
            if (shootRecoverTimer > 0) {
                shootRecoverTimer -= Time.deltaTime;
            }

            if (shootPrepareTimer > 0) {
                shootPrepareTimer -= Time.deltaTime;
            }

            if (losCheckTimer > 0) {
                losCheckTimer -= Time.deltaTime;
            }
            // if (hitStunTimer > 0) {
            //     hitStunTimer -= Time.deltaTime;
            //     if (hitStunTimer <= 0){
            //         hitStunned = false;
            //     }
            // }
            yield return null;
        }
    }
}
