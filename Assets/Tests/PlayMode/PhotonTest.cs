using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;
using Photon.Realtime;

public class PhotonTest {

    [OneTimeSetUp]
    public void SetUp() {
        GameObject obj = new GameObject();
        var l = obj.AddComponent<PhotonTestLobby>();
        l.Connect();
    }

    public GameObject CreatePhotonGameObject() {
        GameObject obj = new GameObject();
        PhotonView pv = obj.AddComponent<PhotonView>();
        PhotonNetwork.AllocateViewID(pv);
        return obj;
    }
}

public class PhotonTestLobby : MonoBehaviourPunCallbacks {

    public void Connect() {
        PhotonNetwork.OfflineMode = true;
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.CreateRoom(
            null,
            new RoomOptions { MaxPlayers = 1 }
        );
    }
}