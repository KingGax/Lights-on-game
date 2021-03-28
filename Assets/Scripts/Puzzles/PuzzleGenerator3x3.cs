using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator3x3 : MonoBehaviour
{

    public List<GameObject> walls;
    // Start is called before the first frame update


    void Awake(){
        if(walls.Count != 9){
            Debug.Log("Incorrect number of walls");
        }
        foreach (GameObject wall in walls){
            wall.GetComponentInChildren<LightableObstacle>().colour = LightableColour.Green;
            //wall.SetActive(false);
        }
    }
    void Start()
    {
        GeneratePuzzle();
        
    }

    bool FindPath(){
        return true;
    }

    void GeneratePuzzle(){
        List<int> validOptions = new List<int>(){0,1,2};
        foreach(int index in validOptions){
            Debug.Log("change light colour");
            LightableObstacle lw = walls[index].GetComponentInChildren<LightableObstacle>();
            lw.colour = LightableColour.Red;
            lw.SetColour();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
