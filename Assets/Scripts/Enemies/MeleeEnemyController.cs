using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class MeleeEnemyController : Enemy {
    public float damage;
    public float detectionThreshold;
    public float minDistance;
    public float engageDistance;
    public float chasingSpeed;
    public float attackingMoveSpeed;

    public bool reactsToPlayerCover;
    public float missedShotReduction;
    float losCheckTimer;
    public float losCheckTimerMax;
    EnemyState enemyState;
    float pathStoppingThreshold = 0.01f;

    enum EnemyState {
        Chasing, //Actively following
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        Attacking, //Swinging weapon
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine("EnemyTimers");
        inStunnableState = true;
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        losCheckTimer = losCheckTimerMax;
        pv = GetComponent<PhotonView>();
    }
    public override void Awake()
    {
        base.Awake();
        if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
            hasPlayerJoined = true;
            SelectTarget();
        }
    }

    void ManageStates(){ //Handles decision logic
        if (aiEnabled) { 
            switch (enemyState) {
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.Chasing:
                    Chasing();
                    break;
                case EnemyState.Attacking:
                    Attacking();
                    break;
                default:
                    break;
            }
        }
    }

    void Update() {
        if (pv == null || !pv.IsMine) return;
        if (!hasPlayerJoined){
            if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
                hasPlayerJoined = true;
                SelectTarget();
            } else {
                return;
            }
        } 
        ManageStates();
    }

    void Patrol() //Handler for idle state - select target and chase player if targeted and in range
    {
        float minDist = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < GlobalValues.Instance.players.Count; i++)
        {
            float distToPlayer = Vector3.Distance(GlobalValues.Instance.players[i].transform.position, transform.position);
            if (distToPlayer < minDist)
            {
                minDist = distToPlayer;
                index = i;
            }
        }
        playerObj = GlobalValues.Instance.players[index];
        if (minDist < detectionThreshold)
        {
            if (losCheckTimer <= 0)
            {
                if (HasPlayerLOS(playerObj, detectionThreshold))
                {
                    ChangeToChasing();
                }
            }
        }

    }

    void ChangeToChasing() { //Setup for chasing state
        enemyState = EnemyState.Chasing;
        agent.speed = chasingSpeed;
        agent.enabled = true;
        SelectTarget();
    }

    void Attacking() { //Handler for melee attack state
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer <= minDistance) {
            agent.destination = agent.transform.position;
        } else {
            agent.destination = playerObj.transform.position;
        }
        
        if (distToPlayer > engageDistance && weapon.CanUse()) {
            ChangeToChasing();
        }
        Vector3 playerDirection = playerObj.transform.position - transform.position;
        playerDirection.y = 0f;
        if (distToPlayer < engageDistance) {
            TurnTowards(playerDirection);
        }
        weapon.Use();
    }

    void ChangeToAttacking() { //Setup for attack state
        enemyState = EnemyState.Attacking;
        agent.speed = attackingMoveSpeed;
        agent.enabled = true;
    }

    void Chasing() { //Handler for chasing state
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        agent.destination = playerObj.transform.position;
        if (distToPlayer < engageDistance) {
            ChangeToAttacking();
        }
    }

    private IEnumerator EnemyTimers() { //Coroutine for enemy timer(s)
        while (true) {
            if (losCheckTimer > 0) {
                losCheckTimer -= Time.deltaTime;
            }

            yield return null;
        }
    }
}