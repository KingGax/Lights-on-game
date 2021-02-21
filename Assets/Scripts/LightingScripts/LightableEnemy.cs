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
    protected void InitialiseEnemyRPC(LightableColour newCol)
    {
        transform.parent.SetParent(GlobalValues.Instance.enemyParent.transform);
        colour = newCol;
        SetColour();
    }
    public void InitialiseEnemy(LightableColour newCol)
    {
        pv.RPC("InitialiseEnemyRPC", RpcTarget.All, newCol);
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