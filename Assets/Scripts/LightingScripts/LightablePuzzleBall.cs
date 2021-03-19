using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightablePuzzleBall : LightableObject
{

    public BouncyBall ball;

    public override void Disappear(){
        ball.ActivateBall();
    }
}