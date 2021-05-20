using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LightsOn.WeaponSystem;

public class ChargerEnemyController : Enemy {

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
    Vector3 savedSpeed = new Vector3(0,0,0);
    public float backoffThreshold;
    float pathStoppingThreshold = 0.01f;
    public float closeToPlayerDistance;
    enum EnemyState {
        Charging, //Charging towards the player
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        ChargeStart, //Preparing to charge
        ChargeEnd, //Charge has ended
        Stunned, //Stunned, eg from colliding with a wall while charging
        Backoff //Started charging too close to enemy, backing off
    }
    // Start is called before the first frame update
    void Start() //Initialise variables and start timers
    {
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        animator = GetComponent<Animator>();
        started = true;
        weaponScript.damage = damage;
        weaponScript.knockback = knockback;
        weaponScript.knockbackDuration = knockbackDuration;
        StartCoroutine("EnemyTimers");
    }

    public override void Awake() //Select target from existing players on wakeup
    {
        base.Awake();
        if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
            hasPlayerJoined = true;
            SelectTarget();
        }
    }

    public void Disappear(){ //store velocity when frozen
        savedSpeed = agent.velocity;
    }

    public void Appear(){ //set velocity to saved velocity on being unfrozen
        agent.velocity = savedSpeed;
        savedSpeed = new Vector3(0,0,0);
    }
    void Patrol() //Handler for idle state - detection logic/targeting logic
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

    void ChangeToChargeStart() //Setup for charge "wind-up"
    {
        chargeStartTimer = chargeStartTimerMax;
        enemyState = EnemyState.ChargeStart;
        SelectTarget();
    }
    void ChargeStart() //State handler for charge "wind-up"; decide whether to charge or back-off. Also includes animation calls
    {
        animator.SetBool("chargeStart", true);
        if (chargeStartTimer <= 0) {
            animator.SetBool("chargeStart", false);
            if (Vector3.Distance(playerObj.transform.position, gameObject.transform.position) < backoffThreshold){
                ChangeToBackingOff();
            } else {
                ChangeToCharging();
            }
        }
    }

    void ChangeToCharging() //Setup for charging state
    {
        inStunnableState = false;
        chargeTimer = chargeTimerMax;
        agent.enabled = true;
        agent.destination = playerObj.transform.position;
        enemyState = EnemyState.Charging;
        weapon.Use();
    }
    void Charge() //Poll player position regularly and update navmesh destination
    {
        if (playerPositionPoll <= 0)
        {
            NavMeshHit destPos;
            NavMesh.SamplePosition(playerObj.transform.position, out destPos, 2f, NavMesh.AllAreas);
            agent.autoBraking = (Vector3.Distance(destPos.position, transform.position) > closeToPlayerDistance); //if closer than threshold, don't autobrake
            agent.destination = destPos.position;
            playerPositionPoll = playerPositionPollMax;
        }
        if (chargeTimer <= 0)
        {
            ChangeToChargeEnd();
        }
    }

    public void ChangeToChargeEnd() //Setup for charge "wind-down"
    {
        weaponScript.Deactivate();
        inStunnableState = true;
        chargeTimer = 0f;
        playerPositionPoll = 0f;
        chargeEndTimer = chargeEndTimerMax;
        agent.enabled = false;
        enemyState = EnemyState.ChargeEnd;
    }
    void ChargeEnd() //State handler for charge "wind-down"
    {
        if (chargeEndTimer <= 0)
        {
            ChangeToPatrolling();
        }
    }

    void ChangeToPatrolling() //Setup for patrolling state (idle state)
    {
        enemyState = EnemyState.Patrolling;
        //do nothing at the moment
    }
    
    void ChangeToBackingOff(){ //Setup for backOff state
        Vector3 backOffPos = gameObject.transform.position + Vector3.Normalize(gameObject.transform.position - playerObj.transform.position) * backoffThreshold * 2; //backoff in opposite direction of player
        NavMeshHit navmeshPos;
        if (NavMesh.SamplePosition(backOffPos, out navmeshPos, 5f, NavMesh.AllAreas)){
            agent.enabled = true;
            agent.destination = navmeshPos.position;
            enemyState = EnemyState.Backoff;
        } else {
            ChangeToPatrolling();
        }
    }

    void BackingOff(){ //Handler for enemy backing away from player if too close
        float dist = agent.remainingDistance;
        if (dist != Mathf.Infinity && agent.remainingDistance <= pathStoppingThreshold) {
            ChangeToChargeStart();
        }
    }

    void ManageStates(){ //Calls appropriate state handler depending on current enemy state
        if (aiEnabled)
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
                case EnemyState.Backoff:
                    BackingOff();
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    /* Decides whether AI processing should be enabled, then calls target selection and state managing logic*/
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
        ManageStates();
    }


    private IEnumerator EnemyTimers() //Coroutine for enemy timers
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
