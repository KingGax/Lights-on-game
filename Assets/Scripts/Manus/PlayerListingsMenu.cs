using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform _content;


    [SerializeField]
    public GameObject _playerListing;

    private Dictionary<string, GameObject> cachedPlayerList = new Dictionary<string, GameObject>();

    void Awake()
    {
        cachedPlayerList.Clear();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentPlayers();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        foreach(KeyValuePair<string, GameObject> elem in cachedPlayerList) {
            Destroy(elem.Value);
        }
        cachedPlayerList.Clear();
    }

    private void GetCurrentPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        foreach(KeyValuePair<int, Player> elem in PhotonNetwork.CurrentRoom.Players) {
            Player player = elem.Value;
            if (cachedPlayerList.ContainsKey(player.UserId)){
                PlayerListingInfo playerInfo = cachedPlayerList[player.UserId].GetComponent<PlayerListingInfo>();
                playerInfo.SetPlayerInfo(player);
            }
            else {
                GameObject listing =  Instantiate(_playerListing,_content);
                PlayerListingInfo playerInfo = listing.GetComponent<PlayerListingInfo>();
                playerInfo.SetPlayerInfo(player);
                cachedPlayerList[player.UserId] = listing;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject listing =  Instantiate(_playerListing,_content);
        PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
        roomInfo.SetPlayerInfo(newPlayer);
        cachedPlayerList[newPlayer.UserId] = listing;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (cachedPlayerList.ContainsKey(otherPlayer.UserId)){
            Destroy(cachedPlayerList[otherPlayer.UserId]);
            cachedPlayerList.Remove(otherPlayer.UserId);
        }
    }
}
