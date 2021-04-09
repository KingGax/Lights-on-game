using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;

public class EnemySpawnInitialiser : MonoBehaviour {
    public PhotonView pv;
    private PhotonView[] childPVs;
    public GameObject enemy;
    private double spawnTime = double.MaxValue;
    private LightColour enemyCol;
    private string parentStr;
    bool spawned = false;

    void Update() {
        if (!spawned && PhotonNetwork.Time > spawnTime) {
            spawned = true;
            SpawnEnemy();
        } else {
            if (enemy == null) {
                foreach (PhotonView childPV in childPVs) {
                    if (childPV.IsMine) {
                        PhotonNetwork.CleanRpcBufferIfMine(childPV);
                    }
                }

                if (pv.IsMine) {
                    PhotonNetwork.CleanRpcBufferIfMine(pv);
                    PhotonNetwork.Destroy(transform.parent.gameObject);
                }
            }
        }
    }

    public void SetupSpawner(LightColour col, double _spawnTime, string parentString) {
        pv.RPC("SetupSpawnerRPC", RpcTarget.AllBufferedViaServer, col, _spawnTime, parentString);
    }

    [PunRPC]
    public void SetupSpawnerRPC(LightColour col, double _spawnTime, string parentString) {
        parentStr = parentString;
        transform.parent.SetParent(GameObject.Find(parentStr).transform);
        spawnTime = _spawnTime;
        enemyCol = col;
    }

    void SpawnEnemy() {
        LightableEnemy enemyScript = enemy.GetComponentInChildren<LightableEnemy>();
        childPVs = enemy.GetComponentsInChildren<PhotonView>();
        enemy.SetActive(true);
        if (enemy != null) {
            enemyScript.InitialiseEnemy(enemyCol, parentStr);
        }
    }
}
