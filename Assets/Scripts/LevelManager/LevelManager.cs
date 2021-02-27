using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wave {
    public Transform centre;
    public float spawnRate;
    public int numEnemies;
}

public class LevelManager : MonoBehaviour {
    public GameObject door;
    public Spawner spawnScript;
    public float spawnTime;
    public GameObject whiteLight;
    public GameObject magentaLight;
    public List<int> enemyWaveNumbers;
    public List<Transform> wavePositions;
    public float spawnRate;
    public List<Wave> waves;
    private GameObject enemyParent;
    private PhotonView pv;
    private LightableObject doorLightable;
    private bool allEnemiesSpawned = false;
    private bool allWavesSpawned = false;
    private int currentWaveCounter = -1;
    // Start is called before the first frame update
    void Start() {
        enemyParent = GlobalValues.Instance.enemyParent;
        pv = gameObject.GetPhotonView();
        doorLightable = door.GetComponentInChildren<LightableObject>();
        if (pv == null || !pv.IsMine) spawnScript.enabled = false;
        StartNewWave();
    }
    [PunRPC]
    public void UnlockDoor() {
        doorLightable.enabled = true;
        whiteLight.SetActive(false);
        magentaLight.SetActive(true);
    }
    // Update is called once per frame
    void Update() {
        if (pv == null || !pv.IsMine) return;
        if (allWavesSpawned) {
            if (CountEnemies() == 0) {
                pv.RPC("UnlockDoor", RpcTarget.All);
            }
        }
        else {
            if (WaveSpawnFinished() && CountEnemies() == 0) {
                StartNewWave();
            }
        }
    }

    void StartNewWave() {
        currentWaveCounter += 1;
        if (currentWaveCounter == enemyWaveNumbers.Count) {
            allWavesSpawned = true;
        }
        else {
            Wave currentWave = new Wave();
            currentWave.centre = wavePositions[currentWaveCounter];
            currentWave.numEnemies = enemyWaveNumbers[currentWaveCounter];
            currentWave.spawnRate = spawnRate;
            spawnScript.SpawnWave(currentWave);
        }
    }

    int CountEnemies() {
        return enemyParent.transform.childCount;
    }

    bool WaveSpawnFinished() {
        return spawnScript.FinishedSpawning();
    }
}
