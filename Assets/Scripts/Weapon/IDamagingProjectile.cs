using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagingProjectile 
{
    public abstract void HitPlayer();
    public abstract float GetDamage();
    public abstract float GetHitstun();
}
