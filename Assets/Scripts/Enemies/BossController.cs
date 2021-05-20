using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LightsOn.LightingSystem;
using Photon.Realtime;

namespace LightsOn.WeaponSystem {
    public class BossController : Enemy , IOnPhotonViewOwnerChange {
        [Header("Bullet/weapon setup")]
        public GameObject bullet; //Bullet prefab reference
        public float bulletSpeed; //Bullet speed magnitude
        public int bulletDamage; //Damage per bullet
        public LightColour bulletColour = LightColour.Red; //current colour of boss bullets
        public List<GameObject> targetGOs; //Target gameobject list (one for each gun)
        public GameObject gunParent; //Parent gameobject of boss guns
        public float gunCircleRadius; //Shot radius for spinning attack
        public float bulletTTL; //Time-to-live of boss projectiles
        public Transform fireOrigin; //Origin
        [Header("Attack probabilities")]
        public float rotatingShotProbability; //Probability of starting rotating fire
        public float repositioningProbability; //Probability of repositioning
        public float aoeProbability; //Probability of choosing AoE attack
        public float missileProbability; //Probability of choosing missile attack
        public float summonProbability; //Probability of summoning minions
        [Header("State timers")]
        public float aoeStartTimerMax; //AoE attack "wind-up" duration
        float aoeStartTimer; //AoE attack "wind-up" timer
        public float aoeEndTimerMax; //AoE attack "wind-down" duration
        float aoeEndTimer; //AoE attack "wind-down" timer
        public float rotatingShotTimerMin; //Spinning bullet attack minimum duration
        public float rotatingShotTimerMax; //Spinning bullet attack maximum duration
        float rotatingShotTimer; //Timer for spinning bullet attack
        public float gunCooldownTimerMax; //Time between shots
        float gunCooldownTimer; //Timer for deciding when to shoot again
        public float changeBulletColTimerMax; //Time between changing bullet colours (when shooting)
        float changeBulletColTimer; //Timer for changing bullet colours
        public float reappearingTimerMax; //Duration of reappearing/reappear attack "wind-up"
        float reappearingTimer; //Timer for reappearing/reappear attack "wind up"
        public float movementCooldownTimerMax; //Time between moving passively
        float movementCooldownTimer; //Timer for passive movement
        EnemyState enemyState; //Current enemy state
        EnemyState prevState; //Previous enemy state
        [Header("Navigation/movement")]
        public float pathStoppingThreshold = 0.5f; //Acceptable distance from navmesh target
        public float walkRadius; //Search radius for walkable areas
        public float normalSpeed; //Standard movement speed
        public float repositionSpeed; //Repositioning movement speed
        public float rotationSpeed; //Rotation speed while using rotating shot attack
        public float currentGunAngle; //Base rotation angle for guns
        [Header("AOE attack setup")]
        public float aoeRadius; //Radius of AoE attack
        public float aoeDamage; //Damage of AoE attack 
        public float reappearDamage; //Damage of reappearing AoE attack
        public float reappearKnockbackMagnitude; //Magnitude of knockback of reappearing AoE attack
        public float reappearKnockbackDuration;
        public float aoeHeight;
        LineRenderer circleLR;
        [Header("Missile attack setup")]
        public int missileDamage;
        public float missileShotsMax;
        float missileShotsFired;
        public float missileTimeBetweenShots;
        public float missileSpeed;
        public float missileTurnSpeed;
        float missileTimer = 0;
        int currentPhase;
        float cmRepProb;
        float cmAOEProb;
        float cmMissileProb;
        float totalProb;
        int flashNum = 10;
        int flashesRemaining = 0;
        float flashTimerMax;
        float flashTimer;
        bool moving = false;
        List<EnemyGun> rotatingGuns;
        LightableBossEnemy lightableBoss;
        Vector3 floorPlane = new Vector3(0,1,0);
        EnemyState lastSentState = EnemyState.Spawning;
        public Animator bossanim;
        public float meleeStartup;
        double stateStartTime;

        private bool isActivated = false;


        enum EnemyState {
            DecisionState, //Base state - deciding what to do
            RotateShooting, //Firing rotating shots
            MissileAttack, //Using missile attack
            Patrolling, //Moving/idle state - hasn't engaged the player yet 
            AOEMeleeStartup, //Area-of-effect melee attack startup
            AOEMelee, //actively attacking
            AOEMeleeRecovery, //recovery from AOE melee attack
            SwarmRepositioning, //Moving during combat
            Reappearing, //Reapearring and performing AoE attack
            Staggered, //Staggered/stunned (after taking a certain amount of damage) [not sure if I want to use this]
            Spawning //For when the enemy initially appears
        }

        void Start() //Initialise values, setup renderers and weapons
        {
            stateStartTime = PhotonNetwork.Time;
            bulletColour = LightColour.Red;
            currentPhase = 0;
            enemyState = EnemyState.Spawning;
            StartCoroutine("EnemyTimers");
            agent = GetComponent<NavMeshAgent>();
            //playerObj = GlobalValues.Instance.localPlayerInstance;
            cmRepProb = rotatingShotProbability + repositioningProbability;
            cmAOEProb = cmRepProb + aoeProbability;
            cmMissileProb = cmAOEProb + missileProbability;
            totalProb = cmMissileProb + summonProbability;
            rotatingGuns = new List<EnemyGun>(gunParent.GetComponentsInChildren<EnemyGun>());
            agent.speed = normalSpeed;
            foreach (EnemyGun g in rotatingGuns) {
                g.bulletSpeed = bulletSpeed;
                g.bullet = bullet;
                g.damage = bulletDamage;
                g.bulletTTL = bulletTTL;
                g.SetColour(bulletColour);
            }

            circleLR = GetComponent<LineRenderer>();
            agent.enabled = true;
            moving = false;
            lightableBoss = GetComponentInChildren<LightableBossEnemy>();
            //GetComponentInChildren<BossHealthBar>().Activate();
        }

        public void OnOwnerChange(Player newOwner, Player previousOwner) {
            if (PhotonNetwork.LocalPlayer == newOwner) {
                HandoverBoss();
            }
        }

        void HandoverBoss() {
            enemyState = EnemyState.SwarmRepositioning;
            ChangeToSwarmReposition();
        }

        void IOnPhotonViewOwnerChangeOnOwnerChange(Player newOwner, Player oldOwner) {
            if (PhotonNetwork.LocalPlayer == newOwner) {
                HandoverBoss();
            }
        }

        //Update is called once per frame
        /*
        Handles AoE attack indicator flashing, as well as passive movement.
        Also calls ManageStates for enemy state handling
        */
        void Update() {
            if(!isActivated) return;
            if (flashesRemaining > 0 && flashTimer <= 0) {
                if (flashesRemaining % 2 == 0) {
                    circleLR.enabled = false;
                } else {
                    circleLR.enabled = true;
                }
                flashesRemaining--;
                flashTimer = flashTimerMax;
            }
            if (pv == null || !pv.IsMine) return;
            if (enemyState != EnemyState.SwarmRepositioning && enemyState != EnemyState.Reappearing && enemyState != EnemyState.AOEMeleeStartup && enemyState != EnemyState.Spawning) {
                if (moving && agent.enabled && agent.remainingDistance < pathStoppingThreshold) {
                    moving = false;
                    movementCooldownTimer = movementCooldownTimerMax;
                }
                if (!moving && movementCooldownTimer <= 0) {
                    agent.enabled = true;
                    Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                    Vector3 finalPosition = hit.position;
                    agent.destination = finalPosition;
                    moving = true;
                }
            } else if (enemyState == EnemyState.Spawning){
                ChangeToSwarmReposition();
            }   
            ManageStates();
        }

        //Handles enemy states each frame/iteration
        void ManageStates() {
            //if (aiEnabled) {
                switch (enemyState) {
                    case EnemyState.DecisionState:
                        float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
                        float p = Random.Range(0, pTotal);
                        MakeDecision(p);
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
                    case EnemyState.SwarmRepositioning:
                        SwarmReposition();
                        break;
                    case EnemyState.Reappearing:
                        ReappearState();
                        break;
                    default:
                        break;
                }
            //} 
        }

        [PunRPC]
        void UpdateStateRPC(EnemyState state) {
            enemyState = state;
        }

        void MakeDecision(float p) { //decides which attack/ability to use next, will not use same ability twice in a row
            float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
            if (p < rotatingShotProbability) {
                if (prevState == EnemyState.RotateShooting) {
                    float q = Random.Range(rotatingShotProbability, pTotal + 0);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToRotateShoot();
                }
            } else if (p < cmRepProb) {
                if (prevState == EnemyState.SwarmRepositioning) {
                    float q = Random.Range(cmAOEProb, pTotal + rotatingShotProbability);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToSwarmReposition();
                }
            } else if (p < cmAOEProb) {
                if (prevState == EnemyState.AOEMeleeStartup) {
                    float q = Random.Range(cmAOEProb, pTotal + cmRepProb);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToAOEMeleeStartup();
                }
            } else {
                if (prevState == EnemyState.MissileAttack) {
                    float q = Random.Range(cmMissileProb, pTotal + cmAOEProb);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToMissileAttack();
                }
            }
        }

        void ChangeToRotateShoot() { //setup for rotating shot attack
            enemyState = EnemyState.RotateShooting;
            prevState = enemyState;
            rotatingShotTimer = Random.Range(rotatingShotTimerMin, rotatingShotTimerMax);
            changeBulletColTimer = changeBulletColTimerMax;
            currentGunAngle = transform.eulerAngles.y;
        }

        void RotateShoot() { //handles rotating shot attack logic (changes bullet colours and calls fire() methods)
            currentGunAngle += (rotationSpeed * Time.deltaTime) % (Mathf.PI * 2);
            float phase = 0f;
            bool canShoot = false;
            bool changeCol = false;
            if (gunCooldownTimer <= 0) {
                gunCooldownTimer = gunCooldownTimerMax;
                canShoot = true;
            }
            if (changeBulletColTimer <= 0) {
                changeBulletColTimer = changeBulletColTimerMax;
                changeCol = true;
                switch (bulletColour) {
                    case (LightColour.Red):
                        bulletColour = LightColour.Green;
                        break;
                    case (LightColour.Green):
                        bulletColour = LightColour.Blue;
                        break;
                    case (LightColour.Blue):
                        bulletColour = LightColour.Red;
                        break;
                    default:
                        bulletColour = LightColour.Red;
                        break;

                }
            }            
            for (int i = 0; i < rotatingGuns.Count; i++) {

                EnemyGun g = rotatingGuns[i];
                if (changeCol) {
                    g.SetColour(bulletColour);
                }
                //float x = transform.position.x + gunCircleRadius * Mathf.Cos(currentGunAngle + phase);
                //float z = transform.position.z + gunCircleRadius * Mathf.Sin(currentGunAngle + phase);
                //g.transform.position = new Vector3(x, g.transform.position.y, z);
                phase += (Mathf.PI * 2) / rotatingGuns.Count;
                if (canShoot) {
                    //targetGOs[i].transform.position = new Vector3(transform.position.x + gunCircleRadius * 3 * Mathf.Cos(currentGunAngle + phase),
                    //g.firePoint.position.y,
                    //transform.position.z + gunCircleRadius * 3 * Mathf.Sin(currentGunAngle + phase));
                    g.SetTarget(targetGOs[i]);
                    g.Use();
                }
            }
            if (rotatingShotTimer <= 0) {
                enemyState = EnemyState.DecisionState;
            }
        }
        private void LateUpdate() { //Rotates guns if in RotateShooting state
            if(enemyState == EnemyState.RotateShooting){
                gunParent.transform.parent.rotation = Quaternion.Euler(new Vector3(0f, currentGunAngle, 0f));
            }
        }

        void ChangeToMissileAttack() { //Setup for missile attack
            enemyState = EnemyState.MissileAttack;
            prevState = enemyState;
            missileShotsFired = 0;
        }

        void MissileAttack() { //Missile attack state handler
            if (missileTimer <= 0) {
                GameObject mis = PhotonNetwork.Instantiate("HomingMissile",transform.position,Quaternion.identity);
                MissileController mc = mis.GetComponent<MissileController>();
                mc.Fire(missileSpeed, missileTurnSpeed, missileDamage);
                missileShotsFired++;
                missileTimer = missileTimeBetweenShots; 
            }
            if (missileShotsFired >= missileShotsMax) {
                enemyState = EnemyState.DecisionState;
            }
            
        }

        void Patrol() {
            //Do nothing
        }

        [PunRPC]
        void ShowCircleRPC(float time){ //RPC for showing AoE indicator
            circleLR.enabled = true;
            flashesRemaining = flashNum;
            flashTimerMax = time;
            flashTimer = flashTimerMax;
            DrawPolygon(100, aoeRadius, new Vector3(fireOrigin.position.x, aoeHeight, fireOrigin.position.z), 0.1f, 0.1f);
        }

        void ShowCircle(float time) { //Shows AoE indicator
            pv.RPC("ShowCircleRPC", RpcTarget.All, time);
            
        }

        void ChangeToAOEMeleeStartup() { //Setup for AoE startup
            enemyState = EnemyState.AOEMeleeStartup;
            prevState = enemyState;
            aoeStartTimer = aoeStartTimerMax;
            agent.enabled = false;
            ShowCircle((aoeStartTimerMax) / (flashNum + 1));
            pv.RPC("StartAOE", RpcTarget.Others, PhotonNetwork.Time, aoeStartTimerMax);
            Invoke("SetMeleeTrigger", aoeStartTimerMax - meleeStartup);
        }

        void AOEMeleeStartup() { //AoE startup handler
            if (aoeStartTimer <= 0) {
                ChangeToAOEMelee();
            }
        }

        void ChangeToAOEMelee() { //Setup for AoE attack proper
            enemyState = EnemyState.AOEMelee;
        }

        [PunRPC]
        protected void StartAOE(double time, float startup) {  
            float dt = (float)(PhotonNetwork.Time - time);
            if (dt > startup) {
                dt = startup;
            }
            if (startup - dt - meleeStartup < 0) {
                SetMeleeTrigger();
            } else {
                Invoke("SetMeleeTrigger", startup - dt - meleeStartup);
            }
            
            Invoke("InvokeAOE", startup - dt);
        }

        void SetMeleeTrigger() {
            bossanim.SetTrigger("Melee");
        }
        void InvokeAOE() {
            DoAOEAttack(aoeDamage, reappearKnockbackMagnitude, reappearKnockbackDuration);
        }

        void DoAOEAttack(float dmg, float knockbackMag, float knockbackDuration) { //Perform AoE attack, apply damage and knockback
            Collider[] cols = Physics.OverlapSphere(transform.position, aoeRadius, GlobalValues.Instance.playerLayer);
            
            if (cols.Length > 0) {
                foreach (Collider col in cols) {
                    HealthSystem.Health h = col.gameObject.GetComponentInChildren<HealthSystem.Health>();
                    PhotonView playerPV = h.gameObject.GetPhotonView();
                    if (playerPV != null && playerPV.IsMine) {
                        h.Damage(dmg, 0f);

                        //PlayerController p = col.gameObject.GetComponentInChildren<PlayerController>();
                        IKnockbackable ks = col.gameObject.GetComponentInChildren<IKnockbackable>();
                        if (ks != null) {
                            Vector3 dir = Vector3.Normalize(col.transform.position - transform.position); // this might need to be changed
                            if (dir == Vector3.zero) {
                                dir = new Vector3(1, 0, 0);
                            }
                            Vector3 flatDir = Vector3.ProjectOnPlane(dir, floorPlane);
                            ks.TakeKnockback(flatDir, knockbackMag, knockbackDuration);
                        }
                    }
                }
            }
        }

        void AOEMelee() { //Handler for AoE attack state
            DoAOEAttack(aoeDamage, reappearKnockbackMagnitude, reappearKnockbackDuration);
            //do attack
            ChangeToAOEMeleeRecovery();
        }
        [PunRPC]
        void DisableCircleRPC(){ //RPC for disabling AoE indicator
            circleLR.enabled = false;
        }
        void ChangeToAOEMeleeRecovery() { //Setup for AoE "wind-down"

            pv.RPC("DisableCircleRPC", RpcTarget.All);
            enemyState = EnemyState.AOEMeleeRecovery;
            aoeEndTimer = aoeEndTimerMax;
        }

        void AOEMeleeRecovery() { //State handler for AoE "wind-down"
            if (aoeEndTimer <= 0) {
                agent.enabled = true;
                enemyState = EnemyState.DecisionState;
            }
        }
        [PunRPC]
        void DisappearRPC(){ //RPC for boss disappearing/dispersing
            GetComponentInChildren<LightableBossEnemy>().ForceDisappear();
        }

        void ChangeToSwarmReposition() { //Setup for boss repositioning
            enemyState = EnemyState.SwarmRepositioning;
            prevState = enemyState;
            NavMeshHit destPos;
            int index = Random.Range(0, GlobalValues.Instance.players.Count);
            playerObj = GlobalValues.Instance.players[index];
            NavMesh.SamplePosition(playerObj.transform.position, out destPos, 2f, NavMesh.AllAreas);
            agent.speed = repositionSpeed;
            agent.destination = destPos.position;
            pv.RPC("DisappearRPC", RpcTarget.AllBufferedViaServer);
            lightableBoss.BossSwarm();
            
        }

        void SwarmReposition() { //State handler for SwarmReposition
            if (agent.remainingDistance <= pathStoppingThreshold) {
                ChangeToReappearState();
            }
        }

        void ChangeToReappearState() { //Setup for reappearing AoE attack
            enemyState = EnemyState.Reappearing;
            reappearingTimer = reappearingTimerMax;
            agent.speed = normalSpeed;
            agent.enabled = false;
            ShowCircle((reappearingTimerMax) / (flashNum + 1));
            pv.RPC("QueueReappearRPC", RpcTarget.AllBufferedViaServer, PhotonNetwork.Time, reappearingTimerMax);
            pv.RPC("StartAOE", RpcTarget.Others,PhotonNetwork.Time,reappearingTimerMax);
            Invoke("SetMeleeTrigger", reappearingTimerMax - meleeStartup);
        }
        [PunRPC]
        void QueueReappearRPC(double time, float startup){
            float dt = (float)(PhotonNetwork.Time - time);
            if (dt > startup) {
                dt = startup;
            }
            if (dt<5) {
                Invoke("InvokeReappear", startup - dt);
            }
        }

        void InvokeReappear() {
            lightableBoss.BossReappear();
        }

        void ReappearState() { //State handler for ReappearState
            if (reappearingTimer <= 0) {
                DoAOEAttack(reappearDamage, reappearKnockbackMagnitude, reappearKnockbackDuration);
                lightableBoss.BossReappear();
                if (aiEnabled){
                    //circleLR.enabled = false;
                    pv.RPC("DisableCircleRPC", RpcTarget.All);
                    enemyState = EnemyState.DecisionState;
                    agent.enabled = true;
                } else {
                    float newReappearTimer = 1.5f;
                    reappearingTimer = newReappearTimer; //get from globalvalues
                    pv.RPC("StartAOE", RpcTarget.Others, PhotonNetwork.Time, reappearingTimer);
                    Invoke("SetMeleeTrigger", reappearingTimer - meleeStartup);
                    ShowCircle(newReappearTimer / (flashNum + 1));
                }
                
            }
        }

        private IEnumerator EnemyTimers() { //Coroutine for various enemy timers
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
                if (gunCooldownTimer > 0) {
                    gunCooldownTimer -= Time.deltaTime;
                }
                if (changeBulletColTimer > 0) {
                    changeBulletColTimer -= Time.deltaTime;
                }
                if (movementCooldownTimer > 0) {
                    movementCooldownTimer -= Time.deltaTime;
                }
                if (flashTimer > 0) {
                    flashTimer -= Time.deltaTime;
                }
                if (missileTimer > 0) {
                    missileTimer -= Time.deltaTime;
                }
                yield return null;
            }
        }
        public override void EnableAI()
        {
            base.EnableAI();
        }

        public override void DisableAI() //change AI flag to disabled without disabling navmeshagent
        {
             if (pv.IsMine) {
                aiEnabled = false;
                //agent.enabled = false;
            }
        }

        void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth) {
            //https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/
            circleLR.startWidth = startWidth;
            circleLR.endWidth = endWidth;
            circleLR.loop = true;
            float angle = 2 * Mathf.PI / vertexNumber;
            circleLR.positionCount = vertexNumber;

            for (int i = 0; i < vertexNumber; i++) {
                Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), 0, 0, -1 * Mathf.Sin(angle * i)),
                                                        new Vector4(0, 1, 0, 0),
                                        new Vector4(Mathf.Sin(angle * i), 0, Mathf.Cos(angle * i), 0),
                                        new Vector4(0, 0, 0, 1));
                Vector3 initialRelativePosition = new Vector3(0, 0, radius);
                circleLR.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));

            }
        }

        public void Activate(){
            isActivated = true;
        }

        
    }
}
