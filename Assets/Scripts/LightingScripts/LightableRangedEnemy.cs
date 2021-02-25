using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableRangedEnemy : LightableEnemy
{
    public EnemyGun gunScript;
    public override void SetColour()
    {
        base.SetColour();
        gunScript.SetColour(colour);
    }
}
