using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;

public class EnemyContainer : MonoBehaviour {
    public int waveNumber;
    public float waveOffset;
    public LightColour enemyColour;
    public GameObject enemyPrefab;
    private Transform enemyParent;
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void StartWave(int waveNum, Transform parent) {
        enemyParent = parent;
        
        if (waveNum == waveNumber) {
            Spawn();
        }
        
    }

    private void Spawn() {
        GameObject entity = PhotonNetwork.InstantiateRoomObject(enemyPrefab.name, transform.position, Quaternion.identity);
        //LightableEnemy lightScript = entity.GetComponentInChildren<LightableEnemy>();
        //lightScript.InitialiseEnemy(enemyColour, enemyParent.gameObject.name);
        EnemySpawnInitialiser spawnScript = entity.GetComponentInChildren<EnemySpawnInitialiser>();
        spawnScript.SetupSpawner(enemyColour, PhotonNetwork.Time + waveOffset, enemyParent.gameObject.name);

    }


}
