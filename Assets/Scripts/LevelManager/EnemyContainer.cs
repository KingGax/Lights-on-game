using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyContainer : MonoBehaviour {
    public int waveNumber;
    public float waveOffset;
    public LightableColour enemyColour;
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
            Invoke("Spawn", waveOffset);
        }
        
    }

    private void Spawn() {
        GameObject entity = PhotonNetwork.Instantiate(enemyPrefab.name, transform.position, Quaternion.identity);
        LightableEnemy lightScript = entity.GetComponentInChildren<LightableEnemy>();
        lightScript.InitialiseEnemy(enemyColour, enemyParent);
    }


}
