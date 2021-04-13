using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleGenerator3x3 : MonoBehaviour
{

    public List<GameObject> walls;

    private System.Random rand;
    // Start is called before the first frame update


    void Awake(){
        rand = new System.Random(DateTime.Now.ToString().GetHashCode());
        if(walls.Count != 9){
            Debug.Log("Incorrect number of walls");
        }
        foreach (GameObject wall in walls){
            //wall.GetComponentInChildren<LightableObstacle>().colour = LightableColour.Green;
            //wall.SetActive(false);
        }

        GeneratePuzzle();
    }
    void Start()
    {
        //GeneratePuzzle();
        
    }

    private (int,int) findCoordinates(int location){
        return (location/3, location%3);
    }

    bool FindPath(HashSet<int> taken, int depth, int location, List<int> path){
        if(depth > 3) return false;

        List<int> validOptions = new List<int>();

        if (depth == 0) {
            validOptions.Add(1);
            validOptions.Add(4);
            validOptions.Add(7);
        } else {
            (int,int) coord = findCoordinates(location);
            for(int i = 0; i < 3; i++){
                if(!taken.Contains(3 * coord.Item1 + i)) validOptions.Add(3 * coord.Item1 + i);
                if(!taken.Contains(coord.Item2 + 3 * i)) validOptions.Add(coord.Item2 + 3 * i);
            }
            if(coord.Item1 == 1 && depth != 1) validOptions.Add(-1);
        }

        while(!(validOptions.Count == 0)){
            int index = rand.Next(validOptions.Count);
            int newLocation = validOptions[index];

            if(newLocation == -1){
                return true;
            }
            taken.Add(newLocation);
            if (FindPath(taken, depth+1, validOptions[index], path)){
                path.Add(newLocation);
                return true;
            }
            taken.Remove(newLocation);
            validOptions.Remove(newLocation);
        }

        return false;
    }

    void RotatePuzzleWall(int location, int rotation){
        walls[location].GetComponentInChildren<LightableObstacle>().colour = LightableColour.Red;
        switch(rotation){
            case 0:
                walls[location].transform.rotation = Quaternion.Euler(0,0,0);
                break;
            case 1:
                walls[location].transform.rotation = Quaternion.Euler(0,45,0);
                break;
            case 2:
                walls[location].transform.rotation = Quaternion.Euler(0,90,0);
                break;
            case 3:
                walls[location].transform.rotation = Quaternion.Euler(0,135,0);
                break;
            
        }
    }

    int findBounceDirection(int location, int next){
        (int,int) locationCoordinates = findCoordinates(location);
        (int,int) nextCoordinates     = findCoordinates(next);

        //Don't @ me
        if(locationCoordinates.Item1 == nextCoordinates.Item1){
            return (locationCoordinates.Item2 > nextCoordinates.Item2 ? 2 : 0);
        } else {
            return (1 + (locationCoordinates.Item1 > nextCoordinates.Item1 ? 2 : 0)); 
        }
    }

    void AdjustPuzzleWalls(List<int> path){
        int incomingDirection = 3;
        int bounceDirection;
        int rotation;
        int location;
        int nextLocation;

        Debug.Log("Adjusting walls");

        for(int i = path.Count - 1; i > 0; i--){
            location = path[i];
            nextLocation = path[i-1];

            bounceDirection = findBounceDirection(path[i],path[i-1]);
            rotation = (bounceDirection - incomingDirection + 4) % 4;
            Debug.Log("Rotation " + rotation + ", Incoming " + incomingDirection + ", Bounce " + bounceDirection);

            RotatePuzzleWall(location, rotation);

            incomingDirection = (bounceDirection + 2) % 4;
        }

        location = path[0];
        bounceDirection = 0;
        rotation = (bounceDirection - incomingDirection + 4) % 4;

        Debug.Log("Rotation " + rotation + ", Incoming " + incomingDirection + ", Bounce " + bounceDirection);

        RotatePuzzleWall(location, rotation);
    }

    void GeneratePuzzle(){

        List<int> validOptions = new List<int>() {1,4,7};
        HashSet<int> taken = new HashSet<int>();
        List<int> path = new List<int>();

        FindPath(taken,0,0,path);

        Debug.Log("path");
        for(int i = 0; i < path.Count; i++){
            Debug.Log(path[i]);
        }

        AdjustPuzzleWalls(path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
