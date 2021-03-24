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
    private Transform _specContent;


    [SerializeField]
    public GameObject _playerListing;

    private Dictionary<string, GameObject> cachedPlayerList = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> cachedSpectatorList = new Dictionary<string, GameObject>();

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
        int counter = 0;
        foreach(KeyValuePair<int, Player> elem in PhotonNetwork.CurrentRoom.Players) {
            counter++;
            Player player = elem.Value;
            if (counter < 2){ 
                if (cachedPlayerList.ContainsKey(player.UserId)){
                    PlayerListingInfo playerInfo = cachedPlayerList[player.UserId].GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, false);
                }
                else {
                    GameObject listing =  Instantiate(_playerListing,_content);
                    PlayerListingInfo playerInfo = listing.GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, false);
                    cachedPlayerList[player.UserId] = listing;
                }
            } else {
                if (cachedSpectatorList.ContainsKey(player.UserId)){
                    PlayerListingInfo playerInfo = cachedPlayerList[player.UserId].GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, true);
                }
                else {
                    GameObject listing =  Instantiate(_playerListing,_specContent);
                    PlayerListingInfo playerInfo = listing.GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, true);
                    cachedPlayerList[player.UserId] = listing;
                }
            }
        }
    }

    void AddToPlayerCount(int amount){ //this should use CAS and hence be network-safe
        Room room = PhotonNetwork.CurrentRoom;
        int playerCount = (int)room.CustomProperties["playerCount"];
        Debug.Log("Original count: " + playerCount);
        ExitGames.Client.Photon.Hashtable expectedVals = new ExitGames.Client.Photon.Hashtable();
        expectedVals.Add("playerCount", playerCount);
        int newPlayerCount  = playerCount  + amount;
        ExitGames.Client.Photon.Hashtable newVals = new ExitGames.Client.Photon.Hashtable();
        newVals.Add("playerCount", newPlayerCount);
        room.SetCustomProperties(newVals, expectedVals);
        StartCoroutine(Printer(0.2f));
        IEnumerator Printer(float delay){
            yield return new WaitForSeconds(delay);
            Room newRoom = PhotonNetwork.CurrentRoom;
            Debug.Log("New count: " + (int)newRoom.CustomProperties["playerCount"]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddToPlayerCount(1);
        if (cachedPlayerList.Count < 2){
            GameObject listing =  Instantiate(_playerListing,_content);
            PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
            roomInfo.SetPlayerInfo(newPlayer, false);
            cachedPlayerList[newPlayer.UserId] = listing;
        } else {
            GameObject listing =  Instantiate(_playerListing,_specContent);
            PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
            roomInfo.SetPlayerInfo(newPlayer, true);
            cachedPlayerList[newPlayer.UserId] = listing;
        }
        
    }

    public void SwapSpecateState(){
        Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;
        Room room = PhotonNetwork.CurrentRoom;
        int maxSpectators = room.MaxPlayers - 2;
        // Debug.Log("Max players: " + room.MaxPlayers);
        // Debug.Log("Attempting swap for player: " + player.NickName);
        if (cachedSpectatorList.Count < maxSpectators && cachedPlayerList.ContainsKey(player.UserId)){
            //Debug.Log("Player found!");
            Destroy(cachedPlayerList[player.UserId]);
            cachedPlayerList.Remove(player.UserId);
            GameObject listing =  Instantiate(_playerListing,_specContent);
            PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
            roomInfo.SetPlayerInfo(player, false);
            cachedSpectatorList[player.UserId] = listing;
            AddToPlayerCount(-1);
        } else if (cachedPlayerList.Count < 2 && cachedSpectatorList.ContainsKey(player.UserId)){
            //Debug.Log("Spectator found!");
            Destroy(cachedSpectatorList[player.UserId]);
            cachedSpectatorList.Remove(player.UserId);
            GameObject listing =  Instantiate(_playerListing,_content);
            PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
            roomInfo.SetPlayerInfo(player, true);
            cachedPlayerList[player.UserId] = listing;
            AddToPlayerCount(1);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        AddToPlayerCount(-1);
        if (cachedPlayerList.ContainsKey(otherPlayer.UserId)){
            Destroy(cachedPlayerList[otherPlayer.UserId]);
            cachedPlayerList.Remove(otherPlayer.UserId);
        } else if (cachedSpectatorList.ContainsKey(otherPlayer.UserId)) {
            Destroy(cachedSpectatorList[otherPlayer.UserId]);
            cachedSpectatorList.Remove(otherPlayer.UserId);
        }
    }
}
