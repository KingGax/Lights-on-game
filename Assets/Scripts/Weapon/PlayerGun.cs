using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : Gun
{
    public MeshRenderer mr;
    protected override void Start() {
        base.Start();
    }
    public override void UnequipWeapon() {
        mr.enabled = false;
    }
    public override void EquipWeapon() {
        mr.enabled = true;
    }
    
}
