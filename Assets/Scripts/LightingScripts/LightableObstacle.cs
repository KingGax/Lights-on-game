using UnityEngine;
using UnityEngine.AI;

namespace LightsOn {
namespace LightingSystem {

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
}}}