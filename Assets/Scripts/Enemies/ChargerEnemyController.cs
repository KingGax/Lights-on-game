using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargerEnemyController : Enemy
{

    public float damage;
    public float knockback;
    public float knockbackDuration;
    bool started = false;
    EnemyState enemyState;
    public float chargeTimerMax;
    float chargeTimer;
    public float chargeStartTimerMax; //maybe rename to chargeWindUpDuration ?
    float chargeStartTimer;
    public float chargeEndTimerMax;
    float chargeEndTimer;
    public float detectionThreshold;
    public float playerPositionPollMax;
    float playerPositionPoll;
    public GameObject weaponParent;
    public ContactWeapon weaponScript;
    enum EnemyState
    {
        Charging, //Charging towards the player
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        ChargeStart, //Preparing to charge
        ChargeEnd, //Charge has ended
        Stunned //Stunned, eg from colliding with a wall while charging
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        started = true;
        weaponScript.damage = damage;
        weaponScript.knockback = knockback;
        weaponScript.knockbackDuration = knockbackDuration;
        StartCoroutine("EnemyTimers");
    }

    public override void Awake()
    {
        base.Awake();
        if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
            hasPlayerJoined = true;
            SelectTarget();
        }
    }

    void Patrol()
    {
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
            ChangeToChargeStart();
        }
    }

    void ChangeToChargeStart()
    {
        chargeStartTimer = chargeStartTimerMax;
        enemyState = EnemyState.ChargeStart;
        SelectTarget();
    }
    void ChargeStart()
    {
        //animation stuff here
        if (chargeStartTimer <= 0)
        {
            ChangeToCharging();
        }
    }

    void ChangeToCharging()
    {
        chargeTimer = chargeTimerMax;
        agent.enabled = true;
        agent.destination = playerObj.transform.position;
        enemyState = EnemyState.Charging;
        weapon.Use();
        //[activate melee weapon]
    }
    void Charge()
    {
        if (playerPositionPoll <= 0)
        {
            agent.destination = playerObj.transform.position;
            playerPositionPoll = playerPositionPollMax;
        }
        if (chargeTimer <= 0)
        {
            ChangeToChargeEnd();
        }
    }

    public void ChangeToChargeEnd()
    {
        weaponScript.Deactivate();
        chargeTimer = 0f;
        playerPositionPoll = 0f;
        //[deactivate melee weapon]
        chargeEndTimer = chargeEndTimerMax;
        agent.enabled = false;
        enemyState = EnemyState.ChargeEnd;
    }
    void ChargeEnd()
    {
        //animation stuff here
        if (chargeEndTimer <= 0)
        {
            ChangeToPatrolling();
        }
    }

    void ChangeToStunned()
    {
        enemyState = EnemyState.Stunned;
        //[disable melee weapon]
    }

    void ChangeToPatrolling()
    {
        enemyState = EnemyState.Patrolling;
        //do nothing at the moment
    }

    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if (!hasPlayerJoined){
            if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
                hasPlayerJoined = true;
                SelectTarget();
            } else {
                return;
            }
        } 
        if (aiEnabled)
        {
            //Debug.Log("Count: " + GlobalValues.Instance.players.Count);
            //playerObj = GlobalValues.Instance.players[0];
            switch (enemyState)
            {
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.ChargeStart:
                    ChargeStart();
                    break;
                case EnemyState.Charging:
                    Charge();
                    break;
                case EnemyState.ChargeEnd:
                    ChargeEnd();
                    break;
                case EnemyState.Stunned:
                    //do nothing at the moment
                    break;
                default:
                    break;
            }
        }
    }


    private IEnumerator EnemyTimers()
    {
        while (true)
        {
            if (aiEnabled)
            {
                if (chargeTimer > 0)
                {
                    chargeTimer -= Time.deltaTime;
                }
                if (chargeStartTimer > 0)
                {
                    chargeStartTimer -= Time.deltaTime;
                }
                if (chargeEndTimer > 0)
                {
                    chargeEndTimer -= Time.deltaTime;
                }
                if (playerPositionPoll > 0)
                {
                    playerPositionPoll -= Time.deltaTime;
                }
            }
            yield return null;
        }
    }
}
