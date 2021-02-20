using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class MeleeEnemyController : Enemy {
    GameObject playerObj;
    public float damage;
    public float detectionThreshold;
    public float minDistance;
    public float swingCooldownMax;
    float swingCooldown;
    bool canSwing;
    public float swingTimeLength;
    float swingTimer;
    public float engageDistance;
    public float chasingSpeed;
    public float attackingMoveSpeed;
    public GameObject weaponParent;
    public BaseMeleeWeapon weaponScript;

    public bool reactsToPlayerCover;
    public float missedShotReduction;
    float losCheckTimer;
    public float losCheckTimerMax;
    EnemyState enemyState;
    float pathStoppingThreshold = 0.01f;
    bool started = false;

    enum EnemyState {
        Chasing, //Actively following
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        Attacking, //Swinging weapon
    }

    // Start is called before the first frame update
    void Start() {
        canSwing = true;
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        started = true;
        losCheckTimer = losCheckTimerMax;
        pv = GetComponent<PhotonView>();
    }

    

    // Update is called once per frame
    void Update() {
        if (pv == null || !pv.IsMine) return;
        playerObj = GlobalValues.Instance.players[0];
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

    void Patrol() {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer < detectionThreshold) {
            if (losCheckTimer <= 0) {
                if (HasPlayerLOS(playerObj,detectionThreshold)) {
                    ChangeToChasing();
                }
            }
        }
    }

    void ChangeToChasing() {
        enemyState = EnemyState.Chasing;
        agent.speed = chasingSpeed;
        agent.enabled = true;
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
        TurnTowards(playerDirection);
        if (weapon.CanUse()) {
            swingCooldown = swingCooldownMax;
            swingTimer = swingTimeLength;
            weapon.Use();
        } 
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
            if (swingCooldown > 0) {
                swingCooldown -= Time.deltaTime;
                if (swingCooldown <= 0) {
                    canSwing = true;
                }
            }

            if (swingTimer > 0) {
                swingTimer -= Time.deltaTime;
                if (swingTimer <= 0) {
                }
            }

            if (losCheckTimer > 0) {
                losCheckTimer -= Time.deltaTime;
            }

            yield return null;
        }
    }
}