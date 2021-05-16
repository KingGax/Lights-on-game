using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;

public class MultiDroneSpawnInitialiser : EnemySpawnInitialiser {

    public List<Transform> dronePositions;
    
    
    protected override void SpawnAnim() {
        foreach(Transform t in dronePositions){
            Vector3 decalPos = new Vector3(t.position.x, t.position.y + spawnDecalYOffset, t.position.z);
            Instantiate(spawnDecal, decalPos, Quaternion.identity);
        }
    }

}
