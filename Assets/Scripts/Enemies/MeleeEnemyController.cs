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
        //playerObj = GlobalValues.Instance.players[0];
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

    void Patrol()
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

    void ChangeToChasing() {
        enemyState = EnemyState.Chasing;
        agent.speed = chasingSpeed;
        agent.enabled = true;
        SelectTarget();
    }

    void Attacking() {
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

    void ChangeToAttacking() {
        enemyState = EnemyState.Attacking;
        agent.speed = attackingMoveSpeed;
        agent.enabled = true;
    }

    void Chasing() {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        agent.destination = playerObj.transform.position;
        if (distToPlayer < engageDistance) {
            ChangeToAttacking();
        }
    }

    private IEnumerator EnemyTimers() {
        while (true) {
            if (losCheckTimer > 0) {
                losCheckTimer -= Time.deltaTime;
            }

            yield return null;
        }
    }
}