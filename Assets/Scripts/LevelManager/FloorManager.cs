using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FloorManager : MonoBehaviour
{
    bool twoPlayers = false;
    int p1RoomNum=0;
    int p2RoomNum=0;
    int numPlayers;
    float startEventTimer = 0f;
    float minEventTimer = 0.4f;
    bool[] roomEventsTriggered;
    public List<RoomObjective> levels;
    public List<Transform> p1SpawnPoints;
    public List<Transform> p2SpawnPoints;
    PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        pv = gameObject.GetPhotonView();
        numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        roomEventsTriggered = new bool[levels.Count];
        for (int i = 0; i < roomEventsTriggered.Length; i++) {
            if (levels[i] != null) {
                roomEventsTriggered[i] = false;
            }
            else {
                roomEventsTriggered[i] = true;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if (numPlayers == GlobalValues.Instance.players.Count) {
            if ((twoPlayers && p1RoomNum == p2RoomNum) || !twoPlayers) {
                if (!roomEventsTriggered[p1RoomNum]) {
                    levels[p1RoomNum].StartObjective();
                    roomEventsTriggered[p1RoomNum] = true;
                }
            }
        }
        
    }

    public Vector3 GetSpawnPoint() {
        if (pv.IsMine) {
            return p1SpawnPoints[p1RoomNum].position;
        }
        else {
            return p2SpawnPoints[p2RoomNum].position;
        }
    }

    public void SetPlayerNum(int numPlayers) {
        if (numPlayers == 1) {
            twoPlayers = false;
        }
        else if (numPlayers == 2) {
            twoPlayers = true;
        }
    }

    public void UpdateLocation(GameObject player, int roomNum) {
        if (twoPlayers) {
            if (player == GlobalValues.Instance.players[1]) {
                p2RoomNum = roomNum;
            }
            else if (player == GlobalValues.Instance.players[0]) {
                p1RoomNum = roomNum;
            }
            else
            {
                Debug.LogError("Non player triggered entrance");
            }
        }
        else if (player == GlobalValues.Instance.players[0]) {
            p1RoomNum = roomNum;
        }
        else {
            Debug.LogError("Non player triggered entrance");
        }
    }
}
