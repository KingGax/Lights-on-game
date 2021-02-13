using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightableEnemy : LightableObject
{
    //public NavMeshObstacle obstacle;
    IEnemy enemy;
    int defaultEnemyLayer;
    int hiddenEnemyLayer;
    void Awake(){
        enemy = gameObject.GetComponentInParent<IEnemy>();
        defaultEnemyLayer = transform.parent.gameObject.layer;
        hiddenEnemyLayer = LayerMask.NameToLayer("HiddenEnemies");
    }
    override public void Appear()
    {
        Debug.Log("LightableEnemy Appear");
        //obstacle.enabled = true;
        //base.Appear();
        enemy.EnableAI();
        transform.parent.gameObject.layer = defaultEnemyLayer;
    }
    override public void Disappear()
    {
        Debug.Log("LightableEnemy Disappear");
        //obstacle.enabled = false;
        //base.Disappear();
        enemy.DisableAI();
        transform.parent.gameObject.layer = hiddenEnemyLayer;
    }

    public override bool CheckNoIntersections()
    {
        return true;
    }

}