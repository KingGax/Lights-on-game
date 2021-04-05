using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class LightableEnemy : LightableMultiObject {

    Enemy enemy;
    int defaultEnemyLayer;
    int hiddenEnemyLayer;
    LayerMask enemyReappearPreventionLayers;
    PhotonView pv;
    private LightableColour initCol;
    private string parentName;
    private bool initialiseOnStart = false;
    public bool usesMeshRenderer = false;

    [PunRPC]
    protected virtual void InitialiseEnemyRPC(LightableColour newCol, string parentName) {
        if (initialised) {
            SetColour(newCol);
            gameObject.GetComponentInParent<EnemyHealth>().InitialiseMaterials();
        }
    }

    public override void Start() {
        overrideMeshRenderer = !usesMeshRenderer;
        base.Start();
        gameObject.GetComponentInParent<EnemyHealth>().InitialiseMaterials();
        if (initialiseOnStart) {
            pv.RPC("InitialiseEnemyRPC", RpcTarget.AllBuffered, initCol, parentName);
        }
    }

    public virtual void InitialiseEnemy(LightableColour newCol, string _parentName)
    {
        if (initialised){
            gameObject.GetComponentInParent<EnemyHealth>().InitialiseMaterials();
            pv.RPC("InitialiseEnemyRPC", RpcTarget.AllBuffered, newCol, _parentName);
        }
        else {
            initCol = newCol;
            parentName = _parentName;
            initialiseOnStart = true;
        }
        
    }

    private void OnEnable() {
        pv = gameObject.GetPhotonView();
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
        if (enemy.weapon != null) enemy.weapon.UnFreeze();
        transform.parent.gameObject.layer = defaultEnemyLayer;
        enemy.GetComponent<EnemyHealth>().InitialiseMaterials();

    }

    override public void Disappear() {
        base.Disappear();
        enemy.DisableAI();
        if (enemy.weapon != null) enemy.weapon.Freeze();
        transform.parent.gameObject.layer = hiddenEnemyLayer;
    }

    public override bool CheckNoIntersections() {
        potentialColliders = enemyReappearPreventionLayers;
        return base.CheckNoIntersections();
    }
}