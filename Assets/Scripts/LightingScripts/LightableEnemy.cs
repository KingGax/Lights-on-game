using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightableEnemy : LightableMultiObject {

    //public NavMeshObstacle obstacle;
    Enemy enemy;
    int defaultEnemyLayer;
    int hiddenEnemyLayer;

    override protected void Awake() {
        enemy = gameObject.GetComponentInParent<Enemy>();
        defaultEnemyLayer = transform.parent.gameObject.layer;
        hiddenEnemyLayer = LayerMask.NameToLayer("HiddenEnemies");
    }

    override public void Appear() {
        base.Appear();
        enemy.EnableAI();
        transform.parent.gameObject.layer = defaultEnemyLayer;
    }

    override public void Disappear() {
        base.Disappear();
        enemy.DisableAI();
        transform.parent.gameObject.layer = hiddenEnemyLayer;
    }

    public override bool CheckNoIntersections() {
        return true;
    }
}