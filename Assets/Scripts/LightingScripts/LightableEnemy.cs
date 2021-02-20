using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightableEnemy : LightableMultiObject {

    //public NavMeshObstacle obstacle;
    Enemy enemy;
    int defaultEnemyLayer;
    int hiddenEnemyLayer;
    LayerMask enemyReappearPreventionLayers;

    override protected void Awake() {
        enemy = gameObject.GetComponentInParent<Enemy>();
        defaultEnemyLayer = transform.parent.gameObject.layer;
        hiddenEnemyLayer = LayerMask.NameToLayer("HiddenEnemies");
        enemyReappearPreventionLayers = (1 << LayerMask.NameToLayer("Player"));
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