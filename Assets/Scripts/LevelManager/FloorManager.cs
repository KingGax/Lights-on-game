using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.AudioSystem;

public class FloorManager : MonoBehaviour
{
    bool twoPlayers = false;
    int p1RoomNum=0; 
    int p2RoomNum=0;
    int numPlayers;
    float startEventTimer = 0f;
    float minEventTimer = 0.4f;
    public bool[] roomEventsTriggered;
    public bool[] objectivesComplete;
    public List<RoomObjective> levels;
    public List<Transform> p1SpawnPoints;
    public List<Transform> p2SpawnPoints;
    public List<NavigationPoint> p1NavPoints;
    public List<NavigationPoint> p2NavPoints;

    //NavigationManager navManager;
    PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        pv = gameObject.GetPhotonView();
        
        numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        twoPlayers = numPlayers == 2;
        roomEventsTriggered = new bool[levels.Count];
        objectivesComplete = new bool[levels.Count];
        for (int i = 0; i < roomEventsTriggered.Length; i++) {
            if (levels[i] != null) {
                roomEventsTriggered[i] = false;
                objectivesComplete[i] = false;
            }
            else {
                roomEventsTriggered[i] = true;
                objectivesComplete[i] = true;
            }
            
        }
        //navManager = GetComponent<NavigationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if (numPlayers == GlobalValues.Instance.players.Count) {
            if ((twoPlayers && p1RoomNum == p2RoomNum) || !twoPlayers) {//main loop checks for whether both players are in the same room
                if (!roomEventsTriggered[p1RoomNum]) {
                    levels[p1RoomNum].StartObjective();
                    roomEventsTriggered[p1RoomNum] = true;
                    pv.RPC("UpdateRoomObjectivesRPC", RpcTarget.AllBufferedViaServer, p1RoomNum);
                }
            }
        }
        
    }

    [PunRPC]
    private void UpdateRoomObjectivesRPC(int roomNum) {
        roomEventsTriggered[roomNum] = true;
    }



    public Vector3 GetSpawnPoint() {
        if (pv.IsMine) {
            //navManager.SetPoints(true);
            return p1SpawnPoints[p1RoomNum].position;
        }
        else {
            //navManager.SetPoints(false);
            return p2SpawnPoints[p2RoomNum].position;
        }
    }

    public bool GetObjectivesTriggered() {
        foreach (bool objective in roomEventsTriggered) {
            if (!objective) {
                return false;
            }
        }
        return true;
    }

    public bool GetObjectivesComplete() {
        foreach (bool objective in objectivesComplete) {
            if (!objective) {
                return false;
            }
        }
        return true;
    }

    public int GetPlayerRoom(bool player1) {
        if (player1) {
            return p1RoomNum;
        }
        return p2RoomNum;
    }

    public void SetPlayerNum(int numPlayers) {
        if (numPlayers == 1) {
            twoPlayers = false;
        }
        else if (numPlayers == 2) {
            twoPlayers = true;
        }
    }

    [PunRPC]
    private void UpdateLocationRPC(bool isFirstPlayer, int roomNum) {
        if (isFirstPlayer) {
            p1RoomNum = roomNum;
        }
        else {
            p2RoomNum = roomNum;
        }
    }

    //This function updates the room number of one of the players, but does not do it if both players are not loaded
    public void UpdateLocation(GameObject player, int roomNum) {
        if (GlobalValues.Instance.p1Spawned && GlobalValues.Instance.p2Spawned) {
            twoPlayers = true;
            numPlayers = 2;
        }
        if (PhotonNetwork.IsMasterClient) {
            if (twoPlayers) {
                if (GlobalValues.Instance.p1Spawned && GlobalValues.Instance.p2Spawned) {
                    if (player == GlobalValues.Instance.players[1]) {
                        //Debug.Log("p1 room local " + p1RoomNum);
                        pv.RPC("UpdateLocationRPC", RpcTarget.AllBufferedViaServer, false, roomNum);
                        //p2RoomNum = roomNum;
                    } else if (player == GlobalValues.Instance.players[0]) {
                        //Debug.Log("p2 room local " + p2RoomNum);
                        pv.RPC("UpdateLocationRPC", RpcTarget.AllBufferedViaServer, true, roomNum);
                        //p1RoomNum = roomNum;
                    } else {
                        Debug.LogError("Non player triggered entrance");
                    }
                } else {
                    Debug.LogError("room triggered before both players loaded");
                }
                
            }
            else if (player == GlobalValues.Instance.players[0]) {
                pv.RPC("UpdateLocationRPC", RpcTarget.AllBufferedViaServer, true, roomNum);
                //p1RoomNum = roomNum;
            }
            else {
                Debug.LogError("Non player triggered entrance");
            }
        } 
    }
}
