using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    
    Rigidbody rb;
    GameObject playerObj;
    public GameObject bullet;
    public float damage;
    public float bulletSpeed;
    public float detectionThreshold;
    public float fireCooldownMax;
    public Transform firePoint;
    float fireCooldown;
    bool canShoot;    
    NavMeshAgent agent;
    public float shootingTimerMax;
    public float engageDistance;
    float shootingTimer;

    float startAngle;
    float endAngle;
    float minX;    
    float maxX;
    float minZ;
    float maxZ;
    public bool reactsToPlayerCover;
    public float missedShotReduction;    
    float losCheckTimer;
    public float losCheckTimerMax;
    EnemyState enemyState;
    float pathStoppingThreshold = 0.01f;
    bool started = false;
    enum EnemyState{
        Shooting, //Actively attacking the player
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        Repositioning, //Moving during combat
        GettingLOS //Trying to find LOS
    }

    // Start is called before the first frame update
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        playerObj = GameObject.Find("Player");
        canShoot = true;
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        GeneratePoint();
        started = true;
    }

    void GeneratePoint(){
        Vector3 playerPos = playerObj.transform.position;
        float vectorDir = AngleDir(gameObject.transform.position - playerPos);
        float playerAngle = Vector3.Angle(Vector3.forward, gameObject.transform.position - playerPos);
        float minAngle = (playerAngle-60);
        float maxAngle = (playerAngle+60);
        float angle = Random.Range(minAngle, maxAngle);
        //angle = playerAngle;
        minX = playerPos.x + engageDistance * vectorDir * Mathf.Sin((minAngle) * Mathf.Deg2Rad);
        minZ = playerPos.z + engageDistance * Mathf.Cos((minAngle) * Mathf.Deg2Rad);
        maxX = playerPos.x + engageDistance * vectorDir* Mathf.Sin((maxAngle) * Mathf.Deg2Rad);
        maxZ = playerPos.z + engageDistance * Mathf.Cos((maxAngle) * Mathf.Deg2Rad);
        
        float x = playerPos.x + engageDistance * vectorDir * Mathf.Sin((angle) * Mathf.Deg2Rad);
        float z = playerPos.z + engageDistance  * Mathf.Cos((angle) * Mathf.Deg2Rad);
        Vector3 dest = new Vector3(x, playerPos.y, z);
        agent.destination = dest;
    }

    float AngleDir(Vector3 targetVec){
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

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
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
    void Patrol()
    {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer < detectionThreshold)
        {
            ChangeToRepositioning();
        }
    }
    void ChangeToShooting()
    {
        Debug.Log("Started shooting");
        if (hasPlayerLOS()){
            agent.enabled = false;
            shootingTimer = shootingTimerMax;
            enemyState = EnemyState.Shooting;
        } else {
            Debug.Log("Getting LOS");
            ChangeToGettingLOS();
        }
    }
    void Shooting()
    {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer <= detectionThreshold && canShoot)
        {
            bool canSeePlayer = hasPlayerLOS();
            if (!canSeePlayer){
                if (reactsToPlayerCover){
                    Debug.Log("Reacting!");
                    ChangeToGettingLOS();
                }
                else{
                    shootingTimer-=missedShotReduction;   
                }
            }
            Vector3 targetVector = playerObj.transform.position - firePoint.position;
            Shoot(targetVector);
        }
        if (shootingTimer <= 0)
        {
            ChangeToRepositioning();
        }
    }
    void ChangeToRepositioning()
    {
        Debug.Log("Started repositioning");
        enemyState = EnemyState.Repositioning;
        agent.enabled = true;
        GeneratePoint();
    }
    void Repositioning()
    {
        // float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        
        // if (distToPlayer <= engageDistance)
        // {
        //     ChangeToShooting();
        // }
        // else
        // {
        //     agent.destination = playerObj.transform.position;
        // }
        float dist=agent.remainingDistance; 
        // Debug.Log(agent.remainingDistance);
        // Debug.Log("Dist: "+dist);
        // Debug.Log("Status: "+agent.pathStatus);
        if (dist!=Mathf.Infinity && agent.remainingDistance<= pathStoppingThreshold){ //agent.pathStatus==NavMeshPathStatus.PathComplete &&
            //path complete. credit: https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
            ChangeToShooting();
        }
    }

    void ChangeToGettingLOS(){
        Debug.Log("Started getting LOS");
        losCheckTimer = losCheckTimerMax;
        agent.enabled = true;
        agent.destination = playerObj.transform.position;
        enemyState = EnemyState.GettingLOS;
        
    }

    void GettingLOS(){
        if (losCheckTimer <= 0)
        {
            Debug.Log("Checking LOS again!");
            if (hasPlayerLOS()){
                agent.enabled = false;
                ChangeToShooting();
            } else{
                //check if stuck
                if (agent.remainingDistance!=Mathf.Infinity && agent.remainingDistance<= pathStoppingThreshold){
                    //path complete. credit: https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
                    ChangeToGettingLOS();
                }
                losCheckTimer = losCheckTimerMax;
            }
        }
    }

    bool hasPlayerLOS(){ //does enemy have line-of-sight on the player?
        LayerMask environmentMask = (1 << LayerMask.NameToLayer("StaticEnvironment")) | (1 << LayerMask.NameToLayer("DynamicEnvironment")) | (1 << LayerMask.NameToLayer("Player"));
        RaycastHit hit;
        bool environmentCheck = Physics.Raycast(firePoint.position, playerObj.transform.position - firePoint.position, out hit, detectionThreshold, environmentMask);
        if (environmentCheck){
            return (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"));
        } else{
            return false;
        }
        
        //return !environmentCheck;
    }
    public void Shoot(Vector3 direction)
    {
        fireCooldown = fireCooldownMax;
        canShoot = false;
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, bulletSpeed,direction);
        Destroy(newBullet, 2.0f);
    }

    private IEnumerator EnemyTimers()
    {
        while (true)
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
                if (fireCooldown <= 0)
                {
                    canShoot = true;  
                }
            }
            if (shootingTimer > 0)
            {
                shootingTimer -= Time.deltaTime;
            }
             if (losCheckTimer > 0)
            {
                losCheckTimer -= Time.deltaTime;
            }
            yield return null;
        }
    }
    private void OnDrawGizmos() {
        if (started){
            Gizmos.DrawLine(playerObj.transform.position, new Vector3(minX, playerObj.transform.position.y, minZ));
            Gizmos.DrawLine(playerObj.transform.position, new Vector3(maxX, playerObj.transform.position.y, maxZ));
        }
    }
}