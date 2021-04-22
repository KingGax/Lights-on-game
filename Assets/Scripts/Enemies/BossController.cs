using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LightsOn.LightingSystem;
namespace LightsOn.WeaponSystem{
public class BossController : Enemy
{
    public GameObject bullet;
    public float bulletSpeed;
    public int bulletDamage;
    LightColour bulletColour;
    public float rotatingShotProbability; //Probability of starting rotating fire
    public float repositioningProbability; //Probability of repositioning
    public float aoeProbability; //Probability of choosing AoE attack
    public float missileProbability; //Probability of choosing missile attack
    public float summonProbability; //Probability of summoning minions
    public float aoeStartTimerMax; 
    float aoeStartTimer;
    public float aoeEndTimerMax;
    float aoeEndTimer;
    public float rotatingShotTimerMin;
    public float rotatingShotTimerMax;
    float rotatingShotTimer;
    public float gunCooldownTimerMax;
    float gunCooldownTimer;
    public float reappearingTimerMax;
    float reappearingTimer;
    public float summonTimerMax;
    float summonTimer;
    public float missileShotsMax;
    float missileShotsFired;
    EnemyState enemyState;
    float pathStoppingThreshold = 0.5f;
    float staggerCount;
    public float staggerCountMax;
    public float rotationSpeed;
    public float currentGunAngle;
    int currentPhase;
    float cmRepProb;
    float cmAOEProb;
    float cmMissileProb;
    float totalProb;
    List<EnemyGun> rotatingGuns;
    public List<GameObject> targetGOs;
    public GameObject gunParent;
    public float gunCircleRadius;
    public float bulletTTL;
    public Transform fireOrigin;

    enum EnemyState {
        DecisionState, //Base state - deciding what to do
        RotateShooting, //Firing rotating shots
        MissileAttack, //Using missile attack
        Patrolling, //Moving/idle state - hasn't engaged the player yet 
        AOEMeleeStartup, //Area-of-effect melee attack startup
        AOEMelee, //actively attacking
        AOEMeleeRecovery, //recovery from AOE melee attack
        SummonAdds, //Currently summoning minions
        SwarmRepositioning, //Moving during combat
        Reappearing, //Reapearring and performing AoE attack
        Staggered //Staggered/stunned (after taking a certain amount of damage) [not sure if I want to use this]
    }

    void Start() //0.7s, 0.59rad/s
    {
        currentPhase = 0; 
        enemyState = EnemyState.DecisionState;
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
        playerObj = GlobalValues.Instance.localPlayerInstance;
        cmRepProb = rotatingShotProbability + repositioningProbability;
        cmAOEProb = cmRepProb + aoeProbability;
        cmMissileProb = cmAOEProb + missileProbability;
        totalProb = cmMissileProb + summonProbability;
        Debug.Log("Yo");
        Debug.Log(gunParent.GetComponentsInChildren<EnemyGun>().Length);
        rotatingGuns = new List<EnemyGun>(gunParent.GetComponentsInChildren<EnemyGun>());
        foreach (EnemyGun g in rotatingGuns){
            g.bulletSpeed = bulletSpeed;
            g.bullet = bullet;
            g.damage = bulletDamage;
            g.bulletTTL = bulletTTL;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        // if (!hasPlayerJoined){
        //     if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
        //         hasPlayerJoined = true;
        //         int index = SelectTarget();
        //         weapon.SetTarget(index);
        //     } else {
        //         return;
        //     }
        // } 
        ManageStates();
    }

    void ManageStates(){
        if (aiEnabled) {
            switch (enemyState) {
                case EnemyState.DecisionState:
                    MakeDecision();
                    break;
                case EnemyState.RotateShooting:
                    RotateShoot();
                    break;
                case EnemyState.MissileAttack:
                    MissileAttack();
                    break;
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.AOEMeleeStartup:
                    AOEMeleeStartup();
                    break;
                case EnemyState.AOEMelee:
                    AOEMelee();
                    break;
                case EnemyState.AOEMeleeRecovery:
                    AOEMeleeRecovery();
                    break;
                case EnemyState.SummonAdds:
                    SummonAdds();
                    break;
                case EnemyState.SwarmRepositioning:
                    SwarmReposition();
                    break;
                case EnemyState.Reappearing:
                    ReappearState();
                    break;
                default:
                    break;
            }
        }
    }

    void MakeDecision(){
        float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
        float p = Random.Range(0, pTotal);
        if (p < rotatingShotProbability){
            ChangeToRotateShoot();
        } else if (p < cmRepProb){
            ChangeToSwarmReposition();
        } else if (p < cmAOEProb){
            ChangeToAOEMeleeStartup();
        } else if (p < cmMissileProb){
            ChangeToMissileAttack();
        } else {
            ChangeToSummonAdds();
        }
    }

    void ChangeToRotateShoot(){
        enemyState = EnemyState.RotateShooting;
        rotatingShotTimer = Random.Range(rotatingShotTimerMin, rotatingShotTimerMax);
    }

    Vector3 GetTargetPosition(Vector3 pos){
        RaycastHit hit;
        Vector3 playerDirection = Vector3.Normalize(pos - fireOrigin.position); //GetPlayerDirection();
        if (Physics.Raycast(fireOrigin.position, playerDirection, out hit, 999f, GlobalValues.Instance.environment | GlobalValues.Instance.playerLayer)){
            return hit.point;
        } else{
            Debug.Log("ERROR!");
            return fireOrigin.position;
        }
    }

    void RotateShoot(){
        currentGunAngle += (rotationSpeed * Time.deltaTime)%(Mathf.PI*2);
        float phase = 0f;
        bool canShoot = false;
        if (gunCooldownTimer <= 0){
            gunCooldownTimer = gunCooldownTimerMax;
            canShoot = true;
        }
        for (int i = 0; i < rotatingGuns.Count; i++){
            EnemyGun g = rotatingGuns[i];
            float x = transform.position.x + gunCircleRadius * Mathf.Cos(currentGunAngle + phase);
            float z = transform.position.z + gunCircleRadius * Mathf.Sin(currentGunAngle + phase);
            g.transform.position = new Vector3(x, g.transform.position.y, z);
            phase += (Mathf.PI*2)/rotatingGuns.Count;
            if (canShoot){
                targetGOs[i].transform.position = GetTargetPosition(g.transform.position);
                g.SetTarget(targetGOs[i]);
                g.Use();
            }
        }
        if (rotatingShotTimer <= 0){
            enemyState = EnemyState.DecisionState;
        }
    }

    void ChangeToMissileAttack(){
        enemyState = EnemyState.MissileAttack;
        missileShotsFired = 0;
    }

    void MissileAttack(){
        enemyState = EnemyState.DecisionState;
    }

    void Patrol(){

    }

    void ChangeToAOEMeleeStartup(){
        enemyState = EnemyState.AOEMeleeStartup;
        aoeStartTimer = aoeStartTimerMax;
    }

    void AOEMeleeStartup(){
        if (aoeStartTimer <= 0){
            ChangeToAOEMelee();
        }
        //visual effect?
    }

    void ChangeToAOEMelee(){
        enemyState = EnemyState.AOEMelee;
    }

    void AOEMelee(){
        //do attack
        ChangeToAOEMeleeRecovery();
    }
    void ChangeToAOEMeleeRecovery(){
        enemyState = EnemyState.AOEMeleeRecovery;
        aoeEndTimer = aoeEndTimerMax;
    }

    void AOEMeleeRecovery(){
        if (aoeEndTimer <= 0){
            enemyState = EnemyState.DecisionState;
        }
    }

    void ChangeToSummonAdds(){
        enemyState = EnemyState.SummonAdds;
        summonTimer = summonTimerMax;
    }

    void SummonAdds(){
        if (summonTimer <= 0){
            enemyState = EnemyState.DecisionState;
        }
        //do nothing
    }

    void ChangeToSwarmReposition(){
        enemyState = EnemyState.SwarmRepositioning;
        NavMeshHit destPos;
        NavMesh.SamplePosition(playerObj.transform.position, out destPos, 2f, NavMesh.AllAreas);
        agent.destination = destPos.position;
    }
    
    void SwarmReposition(){
        if (agent.remainingDistance <= pathStoppingThreshold){
            ChangeToReappearState();
        }
    }

    void ChangeToReappearState(){
        enemyState=EnemyState.Reappearing;
    }

    void ReappearState(){
        if (reappearingTimer <= 0){
            enemyState = EnemyState.DecisionState;
        }
    }

    // Start is called before the first frame update
    

    private IEnumerator EnemyTimers() {
        while (true) {
            if (aoeStartTimer > 0) {
                aoeStartTimer -= Time.deltaTime;
            }
            if (aoeEndTimer > 0) {
                aoeEndTimer -= Time.deltaTime;
            }
            if (rotatingShotTimer > 0) {
                rotatingShotTimer -= Time.deltaTime;
            }
            if (reappearingTimer > 0) {
                reappearingTimer -= Time.deltaTime;
            }
            if (summonTimer > 0) {
                summonTimer -= Time.deltaTime;
            }
            if (gunCooldownTimer > 0){
                gunCooldownTimer -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
}
