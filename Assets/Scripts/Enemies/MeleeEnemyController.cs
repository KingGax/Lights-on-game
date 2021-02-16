using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyController : MonoBehaviour, IEnemy {

    GameObject playerObj;
    public float damage;
    public float detectionThreshold;
    public float minDistance;
    public float swingCooldownMax;
    float swingCooldown;
    bool canSwing;
    public float swingTimeLength;
    float swingTimer;
    NavMeshAgent agent;
    public float engageDistance;
    public float chasingSpeed;
    public float attackingMoveSpeed;
    public GameObject weaponParent;
    public BaseMeleeWeapon weaponScript;
    bool swinging;

    public bool reactsToPlayerCover;
    public float missedShotReduction;
    float losCheckTimer;
    public float losCheckTimerMax;
    EnemyState enemyState;
    float pathStoppingThreshold = 0.01f;
    bool started = false;
    bool enabled = true;

    enum EnemyState {
        Chasing, //Actively following
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        Attacking, //Swinging weapon
    }

    // Start is called before the first frame update
    void Start() {
        //rb = gameObject.GetComponent<Rigidbody>();
        playerObj = GameObject.Find("Player");
        canSwing = true;
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        started = true;
        losCheckTimer = losCheckTimerMax;
        weaponParent.transform.forward = Vector3.up;
    }

    // Update is called once per frame
    void Update() {
        if (enabled) {
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

    public void EnableAI() {
        Debug.Log("AI Enabled");
        enabled = true;
        agent.enabled = true;
    }

    public void DisableAI() {
        Debug.Log("AI Disabled");
        enabled = false;
        agent.enabled = false;
    }

    void Patrol() {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer < detectionThreshold) {
            if (losCheckTimer <= 0) {
                if (hasPlayerLOS()) {
                    ChangeToChasing();
                }
            }
        }
    }

    void ChangeToChasing() {
        enemyState = EnemyState.Chasing;
        agent.speed = chasingSpeed;
        weaponParent.transform.forward = Vector3.up;
        agent.enabled = true;
    }

    void Attacking() {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer <= minDistance) {
            agent.destination = agent.transform.position;
        } else {
            agent.destination = playerObj.transform.position;
        }
        
        if (distToPlayer > engageDistance && !swinging) {
            ChangeToChasing();
        }

        if (swinging) {
            weaponParent.transform.LookAt(playerObj.transform.position);
        } else if (swingCooldown <= 0) {
            swingCooldown = swingCooldownMax;
            swingTimer = swingTimeLength;
            swinging = true;
            weaponScript.Swing(damage,swingTimeLength);
            weaponParent.transform.LookAt(playerObj.transform.position);
        } else {
            weaponParent.transform.forward = Vector3.up;
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

    bool hasPlayerLOS() { //does enemy have line-of-sight on the player?
        LayerMask environmentMask =
            (1 << LayerMask.NameToLayer("StaticEnvironment"))
            | (1 << LayerMask.NameToLayer("DynamicEnvironment")) | (1 << LayerMask.NameToLayer("Player"));
        RaycastHit hit;
        bool environmentCheck = Physics.Raycast(transform.position, playerObj.transform.position - transform.position, out hit, detectionThreshold, environmentMask);
        if (environmentCheck) {
            return (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"));
        } else {
            return false;
        }
        //return !environmentCheck;
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
                    swinging = false;
                }
            }

            if (losCheckTimer > 0) {
                losCheckTimer -= Time.deltaTime;
            }

            yield return null;
        }
    }
}