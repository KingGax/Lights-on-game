using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class LightableEnemy : LightableMultiObject {

    //public NavMeshObstacle obstacle;
    Enemy enemy;
    int defaultEnemyLayer;
    int hiddenEnemyLayer;
    LayerMask enemyReappearPreventionLayers;
    PhotonView pv;

    [PunRPC]
    protected void InitialiseEnemyRPC(LightableColour newCol, string parentName)
    {
        transform.parent.SetParent(GameObject.Find(parentName).transform);
        colour = newCol;        
        if (initialised) {
            SetColour();
            gameObject.GetComponentInParent<EnemyHealth>().InitialiseMaterials();
            Debug.Log("Set colour!");
            
        }
    }
    public override void Start() {
        base.Start();
        gameObject.GetComponentInParent<EnemyHealth>().InitialiseMaterials();
    }
    public void InitialiseEnemy(LightableColour newCol, Transform parent)
    {
        transform.parent.SetParent(parent);
        if (initialised){
            gameObject.GetComponentInParent<EnemyHealth>().InitialiseMaterials();
        }
        Debug.Log("RPC TIME");
        pv.RPC("InitialiseEnemyRPC", RpcTarget.All, newCol, parent.gameObject.name);
    }


    override protected void Awake() {
        pv = gameObject.GetPhotonView();
        enemy = gameObject.GetComponentInParent<Enemy>();
        defaultEnemyLayer = transform.parent.gameObject.layer;
        hiddenEnemyLayer = LayerMask.NameToLayer("HiddenEnemies");
        enemyReappearPreventionLayers = 1 << LayerMask.NameToLayer("Player");
    }

    override public void Appear() {
        base.Appear();
        enemy.EnableAI();
        enemy.weapon.UnFreeze();
        transform.parent.gameObject.layer = defaultEnemyLayer;
        enemy.GetComponent<EnemyHealth>().InitialiseMaterials();

    }

    override public void Disappear() {
        base.Disappear();
        enemy.DisableAI();
        enemy.weapon.Freeze();
        transform.parent.gameObject.layer = hiddenEnemyLayer;
    }

    public override bool CheckNoIntersections() {
        potentialColliders = enemyReappearPreventionLayers;
        return base.CheckNoIntersections();
    }
}