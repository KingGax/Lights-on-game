using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalObjectPool : MonoBehaviour {
    public static LocalObjectPool SharedInstance;
    public List<GameObject> pooledBoids;
    public GameObject boidToPool;
    public int amountToPool;
    void Awake() { SharedInstance = this; }
    void Start() {
        pooledBoids = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++) {
            tmp = Instantiate(boidToPool);
            tmp.SetActive(false); 
            pooledBoids.Add(tmp);
        }
    }

    public GameObject GetPooledBoid() {
        for (int i = 0; i < amountToPool; i++) {
            if (!pooledBoids[i].activeInHierarchy) {
                return pooledBoids[i];
            }
        }
        return null;
    }
}
