using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LightsOn.LightingSystem;

public class RandomEnemiesSetter : MonoBehaviour
{

    public List<GameObject> enemyPrefabs;

    public Transform enemyContainers;

    private System.Random rand;

    public int waves;

    public int enemiesPerWave;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Awake(){
        rand = new System.Random(DateTime.Now.ToString().GetHashCode());
        RandomiseEnemySpawn();
    }

    LightColour getColourFromNum(int num){
        switch(num){
            case 0:
                return LightColour.Red;
            case 1:
                return LightColour.Green;
            case 2:
                return LightColour.Blue;
            default:
                return LightColour.Red;
        }
    }

    void RandomiseEnemySpawn(){
        for(int i = 0; i < waves; i++){
            for(int j = 0; j < enemiesPerWave; j++){
                int enemyNum = rand.Next(enemyPrefabs.Count);
                int colourNum = rand.Next(3);
                Vector3 position = new Vector3(UnityEngine.Random.Range(minX,maxX), 2, UnityEngine.Random.Range(minY,maxY));
                GameObject container = Instantiate(enemyPrefabs[enemyNum], position, Quaternion.identity);
                container.transform.parent = enemyContainers.transform;
                EnemyContainer enemyContainer = container.GetComponent<EnemyContainer>();
                enemyContainer.waveNumber = i;
                enemyContainer.waveOffset = rand.Next(4);
                enemyContainer.enemyColour = getColourFromNum(colourNum);
                
            }
        }
    }
}
