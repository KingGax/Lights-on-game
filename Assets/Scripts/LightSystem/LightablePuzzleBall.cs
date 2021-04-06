using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightsOn {
namespace LightingSystem {

public class LightablePuzzleBall : LightableObject {

    public BouncyBall ball;

    public override void Disappear() {
        Debug.Log("a");
        ball.ActivateBall();
    }
}}}