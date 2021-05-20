using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNextScene : MonoBehaviour
{
    public WinScript winScript;

    public List<string> scenes;

    // Start is called before the first frame update
    void Start(){
        generateRandomNextScene();
    }

    int getSceneIndex(int rand){
        if(rand < 4) return 0;
        else if(rand < 7) return 1;
        else return 2;
    }

    void generateRandomNextScene(){
        int sceneNum = Random.Range(0,10);
        winScript.sceneName = scenes[getSceneIndex(sceneNum)];
    }
}
