using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightableEnemy : LightableObject
{
    //public NavMeshObstacle obstacle;
    IEnemy enemy;
    void Awake(){
        enemy = gameObject.GetComponentInParent<IEnemy>();
    }
    override public void Appear()
    {
        Debug.Log("LightableEnemy Appear");
        //obstacle.enabled = true;
        //base.Appear();
        enemy.EnableAI();
    }
    override public void Disappear()
    {
        Debug.Log("LightableEnemy Disappear");
        //obstacle.enabled = false;
        //base.Disappear();
        enemy.DisableAI();
    }

    public override bool CheckNoIntersections()
    {
        return true;
    }

}