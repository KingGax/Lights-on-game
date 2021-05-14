using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.InteropServices;
using LightsOn.AudioSystem;

public class RoomListingsMenu : MonoBehaviourPunCallbacks {

    [SerializeField]
    private Transform _content;

    [DllImport("__Internal")]
    private static extern void setupVoiceChatUnity(string username, string role);

    [SerializeField]
    public GameObject _roomListing;

    public Dictionary<string, GameObject> cachedRoomList = new Dictionary<string, GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log("ROOM");
        Debug.Log(roomList.Count);
        for (int i=0; i<roomList.Count; i++) {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList) {
                if (cachedRoomList.ContainsKey(info.Name)){
                    Destroy(cachedRoomList[info.Name]);
                    cachedRoomList.Remove(info.Name);
                }
            } else {
                if (cachedRoomList.ContainsKey(info.Name)) {
                    RoomListingInfo roomInfo = cachedRoomList[info.Name].GetComponent<RoomListingInfo>();
                    roomInfo.SetRoomInfo(info);
                } else {
                    GameObject listing =  Instantiate(_roomListing,_content);
                    RoomListingInfo roomInfo = listing.GetComponent<RoomListingInfo>();
                    roomInfo.SetRoomInfo(info);
                    cachedRoomList[info.Name] = listing;
                }
            }
        }
    }

    public override void OnJoinedRoom() {
        RoomInfo info = PhotonNetwork.CurrentRoom;
        if(cachedRoomList.ContainsKey(info.Name)) {
            RoomListingInfo roomInfo = cachedRoomList[info.Name].GetComponent<RoomListingInfo>();
            roomInfo.SetRoomInfo(info);
        } else {
            GameObject listing =  Instantiate(_roomListing,_content);
            RoomListingInfo roomInfo = listing.GetComponent<RoomListingInfo>();
            roomInfo.SetRoomInfo(info);
            cachedRoomList[info.Name] = listing;
        }
    }

    public override void OnDisconnected(DisconnectCause cause) {
        cachedRoomList.Clear();
    }
}
