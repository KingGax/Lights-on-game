using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LightsOn.LightingSystem;
using LightsOn.AudioSystem;

public class EnemyController : Enemy {

    public GameObject bullet;
    public float bulletSpeed;
    public float detectionThreshold;
    public float shootingTimerMax;
    public float engageDistance;
    float shootingTimer;
    LightColour bulletColour;
    float startAngle;
    float endAngle;
    float minX;
    float maxX;
    float minZ;
    float maxZ;
    public bool reactsToPlayerCover;
    public float missedShotReduction;
    float hitStunTimer;
    float losCheckTimer;
    public float losCheckTimerMax;
    EnemyState enemyState;
    float pathStoppingThreshold = 0.01f;
    bool hitStunned = false;

    enum EnemyState {
        Shooting, //Actively attacking the player
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        Repositioning, //Moving during combat
        GettingLOS //Trying to find LOS
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        hitStunned = false;
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

    public void SetBulletColour(LightColour col) {
        bulletColour = col;
    }

    void GeneratePoint() { //generate point on circle of radius engageDistance, +- 60 degrees from angle between player and enemy
        Vector3 playerPos = playerObj.transform.position;
        float vectorDir = AngleDir(gameObject.transform.position - playerPos);
        float playerAngle = Vector3.Angle(Vector3.forward, gameObject.transform.position - playerPos);
        float minAngle = (playerAngle - 60);
        float maxAngle = (playerAngle + 60);
        float angle = Random.Range(minAngle, maxAngle);
        minX = playerPos.x + engageDistance * vectorDir * Mathf.Sin((minAngle) * Mathf.Deg2Rad);
        minZ = playerPos.z + engageDistance * Mathf.Cos((minAngle) * Mathf.Deg2Rad);
        maxX = playerPos.x + engageDistance * vectorDir * Mathf.Sin((maxAngle) * Mathf.Deg2Rad);
        maxZ = playerPos.z + engageDistance * Mathf.Cos((maxAngle) * Mathf.Deg2Rad);

        float x = playerPos.x + engageDistance * vectorDir * Mathf.Sin((angle) * Mathf.Deg2Rad);
        float z = playerPos.z + engageDistance * Mathf.Cos((angle) * Mathf.Deg2Rad);
        Vector3 dest = new Vector3(x, playerPos.y, z);
        agent.destination = dest;
    }

    float AngleDir(Vector3 targetVec) {
        //thank you https://forum.unity.com/threads/how-to-get-a-360-degree-vector3-angle.42145/
        Vector3 perp = Vector3.Cross(Vector3.forward, targetVec);
        float dir = Vector3.Dot(perp, Vector3.up);
        if (dir > 0.0) {
            return 1.0f;
        } else if (dir < 0.0) {
            return -1.0f;
        } else {
            return 0.0f;
        }
    }

    void ManageStates(){ //Handles decision logic
        if (aiEnabled) {
            switch (enemyState) {
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.Repositioning:
                    Repositioning();
                    break;
                case EnemyState.Shooting:
                    Shooting();
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
    void Update() {
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
        ManageStates();
    }

    void Patrol() { //Idle state handler - select target then change to repositioning if in range
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
        //}
        if (minDist < detectionThreshold)
        {
            ChangeToRepositioning();
        }
    }

    public override void RequestHitStun(float duration) //increase shot cooldown
    {
        if (inStunnableState) {
            hitStunned = true;
            hitStunTimer = duration;
            weapon.cooldownLeft+=duration;
            base.RequestHitStun(duration);  
        }
    }
    

    void ChangeToShooting() { //Setup for shooting state
        if (HasPlayerLOS(playerObj, detectionThreshold)) {
            agent.enabled = false;
            shootingTimer = shootingTimerMax;
            enemyState = EnemyState.Shooting;
        } else {
            ChangeToGettingLOS();
        }
    }

    /*
    State handler for shooting/ranged attack
    Checks LoS and reacts accordingly
    */
    void Shooting() { 
        if (!hitStunned) {
            float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
            if (distToPlayer <= detectionThreshold) {
                bool canSeePlayer = HasPlayerLOS(playerObj, detectionThreshold);
                if (!canSeePlayer) {
                    if (reactsToPlayerCover) {
                        ChangeToGettingLOS();
                    } else {
                        Vector3 playerDirection = playerObj.transform.position - transform.position;
                        playerDirection.y = 0f;
                        TurnTowards(playerDirection);
                        if (weapon.Use()) {
                            shootingTimer -= missedShotReduction;
                            AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXEnemyGunfire, transform.position, gameObject);
                        }
                    }
                } else {
                    Vector3 playerDirection = playerObj.transform.position - transform.position;
                    playerDirection.y = 0f;
                    TurnTowards(playerDirection);
                    if (weapon.Use()) {
                        //shootingTimer -= missedShotReduction;
                    }
                }
            }

            if (shootingTimer <= 0) {
                ChangeToRepositioning();
            }
        }
    }

    void ChangeToRepositioning() { //Setup for repositioning
        enemyState = EnemyState.Repositioning;
        agent.enabled = true;
        int index = SelectTarget();
        weapon.SetTarget(index);
        GeneratePoint();
    }

    void Repositioning() { //repositioning state handler
        float dist = agent.remainingDistance;
        if (dist != Mathf.Infinity && agent.remainingDistance <= pathStoppingThreshold) {
            //path complete. credit: https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
            ChangeToShooting();
        }
    }

    void ChangeToGettingLOS() { //setup for finding LoS
        losCheckTimer = losCheckTimerMax;
        agent.enabled = true;
        agent.destination = playerObj.transform.position;
        enemyState = EnemyState.GettingLOS;

    }

    void GettingLOS() { //State handler for GettingLOS
        if (losCheckTimer <= 0) {
            if (HasPlayerLOS(playerObj, detectionThreshold)) {
                agent.enabled = false;
                ChangeToShooting();
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

    private IEnumerator EnemyTimers() { //Coroutine for enemy timers
        while (true) {
            if (shootingTimer > 0) {
                shootingTimer -= Time.deltaTime;
            }

            if (losCheckTimer > 0) {
                losCheckTimer -= Time.deltaTime;
            }
            if (hitStunTimer > 0) {
                hitStunTimer -= Time.deltaTime;
                if (hitStunTimer <= 0){
                    hitStunned = false;
                }
            }
            yield return null;
        }
    }
}
