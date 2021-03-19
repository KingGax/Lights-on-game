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
    float shotFlashDuration;
    float losCheckTimer;
    public float losCheckTimerMax;
    float pathStoppingThreshold = 0.01f;
    public float maxLaserDistance;
    LineRenderer laser;
    bool hasFlashed = false;
    int flashNum = 7;
    int flashesRemaining = 0;
    float flashTimerMax = 0.1f;
    float flashTimer;
    GameObject targetGO;
    Transform targetTF;
    bool RPCLaserEnabled = false;
    
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
        shotFlashDuration = flashNum * flashTimerMax + 0.02f;
        targetGO = new GameObject();
        targetTF = targetGO.transform;
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
        if (flashesRemaining > 0 && flashTimer <= 0){
            
            if (flashesRemaining % 2 == 0){
                // mat.SetColor("_BaseColor", Color.red);     
                // mat.SetColor("_EmissionColour", Color.white); 
                laser.enabled = false;         
            } else {
                laser.enabled = true; 
                // mat.SetColor("_BaseColor", baseCol);
                // mat.SetColor("_EmissionColour", emisCol);
            }
            flashesRemaining--;
            flashTimer = flashTimerMax;
        }
        if (pv == null) return;
        if (!pv.IsMine){
            if (RPCLaserEnabled){
                TrackLaser(false);
            } else {
                return;
            }
        }
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

    public override void RequestHitStun(float duration)
    {
        if (inStunnableState) {
            //Debug.Log("Float stunned");
            //hitStunned = true;
            //hitStunTimer = duration;
            if (shootPrepareTimer >0){
                shootPrepareTimer += duration;
            } else if (shootRecoverTimer > 0){
                shootRecoverTimer += duration;
            }
           
            base.RequestHitStun(duration);  
        }
    }

    Vector3 GetLaserPosition(){
        RaycastHit hit;
        Vector3 playerDirection = transform.forward; //GetPlayerDirection();
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
    [PunRPC]
    protected void ChangeLaserStateRPC(bool isStarting){
        RPCLaserEnabled = isStarting;
        laser.enabled = isStarting;
        if (isStarting){
            Vector3 hitPos = GetLaserPosition();
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, hitPos);
            targetTF.position = hitPos;
        }
    }

    void ChangeToShootPrepare(){
        enemyState = EnemyState.ShootPrepare;
        hasFlashed = false;
        //activate laser
        laser.enabled = true;
        Vector3 hitPos = GetLaserPosition();
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, hitPos);
        pv.RPC("ChangeLaserStateRPC", RpcTarget.All, true);
        targetTF.position = hitPos;
        shootPrepareTimer = shootPrepareTimerMax;
        Debug.Log("Preparing shot");
    }

    void TrackLaser(bool isMaster){
        Vector3 playerDirection = GetPlayerDirection();
        if (isMaster){
            TurnTowards(playerDirection); 
        }
        Vector3 hitPos = GetLaserPosition();
        laser.SetPosition(1, hitPos);
        targetTF.position = hitPos;
        if (shootPrepareTimer <= shotFlashDuration && !hasFlashed){
            hasFlashed = true;
            inStunnableState = false; //enemy can't be hitstunned while about to shoot
            flashesRemaining = flashNum;
        }
    }

    void ShootPrepare(){
        TrackLaser(true);
        if (shootPrepareTimer <= 0){
            ChangeToShooting();
        }
    }

    void ChangeToShooting(){
        //if (HasPlayerLOS(playerObj, detectionThreshold)) {
        agent.enabled = false;
        enemyState = EnemyState.Shooting;
        laser.enabled = false;
        pv.RPC("ChangeLaserStateRPC", RpcTarget.All, false);
        // } else {
        //     ChangeToGettingLOS();
        // }
    }

    void Shooting(){
        Vector3 playerDirection = playerObj.transform.position - transform.position;
        playerDirection.y = 0f;
        TurnTowards(playerDirection);
        weapon.SetTarget(targetGO);
        weapon.Use();
        ChangeToShootRecover();
    }

    void ChangeToShootRecover(){
        enemyState = EnemyState.ShootRecover;
        shootRecoverTimer = shootRecoverTimerMax;
        inStunnableState = true;
        agent.enabled = true;
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

            if (flashTimer > 0) {
                flashTimer -= Time.deltaTime;
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
