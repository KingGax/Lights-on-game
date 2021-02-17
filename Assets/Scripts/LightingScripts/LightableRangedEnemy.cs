using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableRangedEnemy : LightableEnemy
{
    public override void SetColour()
    {
        base.SetColour();
        EnemyController rec = transform.parent.GetComponent<EnemyController>();
        rec.SetBulletColour(colour);
    }
}
