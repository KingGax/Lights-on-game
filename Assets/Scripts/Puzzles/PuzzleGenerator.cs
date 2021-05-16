using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleGenerator : MonoBehaviour
{
    public List<GameObject> walls;

    private System.Random rand;

    [SerializeField]
    private int size;

    [SerializeField]
    private int enter;

    [SerializeField]
    private int exit;

    [SerializeField]
    private int maxBounces;

    [SerializeField]
    private int minBounces;

    [Range(0.0f, 1.0f)]
    public float omitProbability;


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

    private (int,int) findCoordinates(int location){
        return (location/size, location%size);
    }

    bool FindPath(HashSet<int> taken, int depth, int location, List<int> path){
        if(depth > maxBounces) return false;

        List<int> validOptions = new List<int>();

        if (depth == 0) {
            for(int i = 0; i < size; i++){
                validOptions.Add(exit + size*i);
            }
        } else {
            (int,int) coord = findCoordinates(location);
            for(int i = 0; i < size; i++){
                if(!taken.Contains(size * coord.Item1 + i)) validOptions.Add(size * coord.Item1 + i);
                if(!taken.Contains(coord.Item2 + size * i)) validOptions.Add(coord.Item2 + size * i);
            }
            if(depth == 1 && location == exit + size * enter){
                for(int i = 0; i < exit; i++){
                    if(validOptions.Contains(i + size * enter)) validOptions.Remove(i + size * enter);
                }
            }
            if(coord.Item1 == enter && depth != 1 && depth >= minBounces) validOptions.Add(-1);
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
        //walls[location].GetComponentInChildren<LightableObstacle>().colour = LightableColour.Red;
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
            return (locationCoordinates.Item2 > nextCoordinates.Item2 ? 0 : 2);
        } else {
            return (1 + (locationCoordinates.Item1 > nextCoordinates.Item1 ? 2 : 0)); 
        }
    }

    void AdjustPuzzlePathWalls(List<int> path){
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
            rotation = (bounceDirection + incomingDirection) % 4;
            Debug.Log("Rotation " + rotation + ", Incoming " + incomingDirection + ", Bounce " + bounceDirection);

            RotatePuzzleWall(location, rotation);

            incomingDirection = (bounceDirection + 2) % 4;
        }

        location = path[0];
        bounceDirection = 0;
        rotation = (bounceDirection + incomingDirection) % 4;

        Debug.Log("Rotation " + rotation + ", Incoming " + incomingDirection + ", Bounce " + bounceDirection);

        RotatePuzzleWall(location, rotation);
    }

    void AdjustPuzzleNonPathWalls(HashSet<int> taken){
        for(int i = 0; i < size * size; i++){
            if(!taken.Contains(i)){
                bool condition = rand.Next(100) < (int)Math.Floor(omitProbability*100);
                if(condition){
                    walls[i].SetActive(false);
                } else {
                    int rotation;
                    if(i == exit + size * enter) rotation = rand.Next(3);
                    else rotation = rand.Next(4);

                    RotatePuzzleWall(i,rotation);
                }
            }
        }
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

        AdjustPuzzlePathWalls(path);
        AdjustPuzzleNonPathWalls(taken);
    }
}