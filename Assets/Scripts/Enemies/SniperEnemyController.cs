using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LightsOn.AudioSystem;

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

    void ManageStates(){ //Handles decision logic for AI states
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

    // Update is called once per frame
    /*
    Handles indicator flashing, target acquisition and calls state decision logic
    */
    void Update()
    {
        if (flashesRemaining > 0 && flashTimer <= 0){
            
            if (flashesRemaining % 2 == 0){ 
                laser.enabled = false;         
            } else {
                laser.enabled = true; 
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
        ManageStates();
        
    }

    public override void RequestHitStun(float duration) //if indicator is not flashing, increase current shot preparation/recovery time
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

    Vector3 GetLaserPosition(){ //get endpoint for laser sight/shot indicator
        RaycastHit hit;
        Vector3 playerDirection = transform.forward;
        if (Physics.Raycast(transform.position, playerDirection, out hit, maxLaserDistance, GlobalValues.Instance.environment | GlobalValues.Instance.playerLayer)){
            return hit.point;
        } else{
            return transform.position;
        }
    }

    Vector3 GetPlayerDirection(){ //get planar vector to targeted player
        Vector3 playerDirection = playerObj.transform.position - transform.position;
        playerDirection.y = 0f;
        return playerDirection;
    }

    void Patrol() { //idle state - retargeting and LOS decision logic
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
    protected void ChangeLaserStateRPC(bool isStarting){ //RPC for changing laser flash state
        RPCLaserEnabled = isStarting;
        laser.enabled = isStarting;
        if (isStarting){
            Vector3 hitPos = GetLaserPosition();
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, hitPos);
            targetTF.position = hitPos;
            hasFlashed = false;
            shootPrepareTimer = shootPrepareTimerMax;
        }
    }

    void ChangeToShootPrepare(){ //Setup for shot preparation
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
    }

    void TrackLaser(bool isMaster){ //logic for moving laser as enemy rotates
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

    void ShootPrepare(){ //State handler for shot preparation
        TrackLaser(true);
        if (shootPrepareTimer <= 0){
            ChangeToShooting();
        }
    }

    void ChangeToShooting(){ //Setup for shooting state proper
        agent.enabled = false;
        enemyState = EnemyState.Shooting;
        laser.enabled = false;
        pv.RPC("ChangeLaserStateRPC", RpcTarget.All, false);
    }

    void Shooting(){ //State handler for shooting
        Vector3 playerDirection = playerObj.transform.position - transform.position;
        playerDirection.y = 0f;
        TurnTowards(playerDirection);
        weapon.SetTarget(targetGO);
        weapon.Use();
        AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXEnemyGunfire, transform.position, gameObject);
        ChangeToShootRecover();
    }

    void ChangeToShootRecover(){ //Setup for shot recovery
        enemyState = EnemyState.ShootRecover;
        shootRecoverTimer = shootRecoverTimerMax;
        inStunnableState = true;
        agent.enabled = true;
    }

    void ShootRecover(){ //Shot recovery/cooldown handler
        if (shootRecoverTimer <= 0){
            ChangeToGettingLOS();
        }
    }

    void ChangeToGettingLOS(){ //Setup for finding LOS/repositioning state
        int index = SelectTarget();
        weapon.SetTarget(index);
        losCheckTimer = losCheckTimerMax;
        agent.enabled = true;
        agent.destination = playerObj.transform.position;
        enemyState = EnemyState.GettingLOS;
    }

    void GettingLOS(){ //FindLOS/reposition state - reposition if cannot currently see the player
        if (losCheckTimer <= 0) {
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

    private IEnumerator EnemyTimers() { //Coroutine for AI-related timers
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
            yield return null;
        }
    }
}
