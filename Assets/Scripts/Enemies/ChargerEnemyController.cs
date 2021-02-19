using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargerEnemyController : MonoBehaviour, IEnemy
{

    Rigidbody rb;
    GameObject playerObj;
    NavMeshAgent agent;
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
        playerObj = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        started = true;
        weaponScript.damage = damage;
        weaponScript.knockback = knockback;
        weaponScript.knockbackDuration = knockbackDuration;
        StartCoroutine("EnemyTimers");
    }

    void Patrol()
    {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer < detectionThreshold)
        {
            ChangeToChargeStart();
        }
    }

    void ChangeToChargeStart()
    {
        chargeStartTimer = chargeStartTimerMax;
        enemyState = EnemyState.ChargeStart;
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
        weaponScript.Activate();
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
        if (enabled)
        {
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

    public void EnableAI()
    {
        agent.enabled = true;
        enabled = true;
    }

    public void DisableAI()
    {
        agent.enabled = false;
        enabled = false;
    }

    private IEnumerator EnemyTimers()
    {
        while (true)
        {
            if (enabled)
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
