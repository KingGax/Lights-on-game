using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace LightsOn {
namespace LightingSystem {
public class LightableTargetDummy : LightableEnemy {

    public Transform parentTransform;
    public bool useParentTransform;

    // [PunRPC]
    // protected override void InitialiseEnemyRPC(LightColour newCol, string parentName) {
    //     /*if (useParentTransform) {
    //         parentTransform.SetParent(GameObject.Find(parentName).transform);
    //     }
    //     else {
    //         transform.parent.SetParent(GameObject.Find(parentName).transform);
    //     }*/
    // }
}}}