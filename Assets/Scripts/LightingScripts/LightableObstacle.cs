using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightableObstacle : LightableObject {

    public NavMeshObstacle obstacle;

    override public void Appear() {
        obstacle.enabled = true;
        base.Appear();
    }

    override public void Disappear() {
        obstacle.enabled = false;
        base.Disappear();
    }
}