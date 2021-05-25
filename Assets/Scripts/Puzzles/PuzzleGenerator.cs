using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//This is a generalised puzzle generation class
public class PuzzleGenerator : MonoBehaviour
{
    public List<GameObject> walls;

    private System.Random rand;


    [SerializeField]
    private int size; //Side of the square grid

    [SerializeField]
    private int enter; //Enter row in integer index starting from 0

    [SerializeField]
    private int exit; //Exit column in integer starting from 0

    // !!!!!! The puzzle assumes the following arrangement: !!!!!!!!!!!!
    //          w   w  ....  size*size - 1
    // enter -> w   w  ....  w
    //          .   .  ....  .
    //          .   .  ....  .
    //          0   1  ....  size
    //                  ^
    //                 exit

    [SerializeField]
    private int maxBounces; //Sets the max length of the found path

    [SerializeField]
    private int minBounces; //Sets the min length of the found path

    [Range(0.0f, 1.0f)]
    public float omitProbability; //Sets the probability a non-path wall is removed 


    void Awake(){
        rand = new System.Random(DateTime.Now.ToString().GetHashCode());

        GeneratePuzzle();
    }

    //Finds the coordinates of the ball as a tuple
    private (int,int) findCoordinates(int location){
        return (location/size, location%size);
    }

    //Recursive algorithm to find a valid path for the ball
    bool FindPath(HashSet<int> taken, int depth, int location, List<int> path){
        if(depth > maxBounces) return false;

        //Find all valid options for the next step in the path
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

        //Pick a random option and recurse
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

    //Adjusts the wall rotation based on an int input
    void RotatePuzzleWall(int location, int rotation){
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

    //Finds the integer encoding of the direction a ball has to travel in after bouncing towards $next from $location
    //Directions: x 1 x
    //            0 w 2
    //            x 3 x 
    // where w is the wall at $location.
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

    //Adjusts the wall rotations along a path
    void AdjustPuzzlePathWalls(List<int> path){
        int incomingDirection = 3;
        int bounceDirection;
        int rotation;
        int location;
        int nextLocation;


        for(int i = path.Count - 1; i > 0; i--){
            location = path[i];
            nextLocation = path[i-1];

            bounceDirection = findBounceDirection(path[i],path[i-1]);
            rotation = (bounceDirection + incomingDirection) % 4;

            RotatePuzzleWall(location, rotation);

            incomingDirection = (bounceDirection + 2) % 4;
        }

        location = path[0];
        bounceDirection = 0;
        rotation = (bounceDirection + incomingDirection) % 4;

        RotatePuzzleWall(location, rotation);
    }

    //Adjusts the walls that are not part of the picked path
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

    //Generates a puzzle
    void GeneratePuzzle(){

        List<int> validOptions = new List<int>() {1,4,7};
        HashSet<int> taken = new HashSet<int>();
        List<int> path = new List<int>();

        FindPath(taken,0,0,path);

        AdjustPuzzlePathWalls(path);
        AdjustPuzzleNonPathWalls(taken);
    }
}
