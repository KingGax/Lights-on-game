using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockbackable {
    bool isTakingKnockback{
        get;
        set;
    }
    void TakeKnockback(Vector3 dir, float magnitude, float duration);
}
