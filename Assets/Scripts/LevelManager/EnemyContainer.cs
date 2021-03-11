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
    PhotonView pv;
    // Start is called before the first frame update
    void Start() {
        pv = gameObject.GetPhotonView();
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void StartWave(int waveNum, Transform parent) {
        if (pv == null || !pv.IsMine) return;
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
