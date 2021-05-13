using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.WeaponSystem;

public class StartBossFight : AfterDialogue
{

    [SerializeField]
    private BossController boss;
    
    public override void Effect(){
        Debug.Log("effect");
        boss.Activate();
    }
}
