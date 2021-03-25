using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private Transform _specContent;

    public Color readyColour = Color.green;
    public Color unreadyColour = Color.red;
    public Color specColour = Color.black;


    [SerializeField]
    public GameObject _playerListing;
    PhotonView pv;
    int readyPlayers = 0;
    public Button spectateButton;

    Dictionary<string, GameObject> cachedPlayerList = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> cachedSpectatorList = new Dictionary<string, GameObject>();

    public float readyCooldownMax;
    float readyCooldown;
    public float spectateCooldownMax;
    float spectateCooldown;

    void Awake()
    {
        cachedPlayerList.Clear();
        pv = GetComponent<PhotonView>();
        StartCoroutine("SyncedLobbyTimers");
        if (!PhotonNetwork.IsMasterClient) {
            GameObject lobby = GameObject.Find("Lobby");
            transform.SetParent(lobby.transform);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentPlayers();
    }

    public bool isReady(){
        Room room = PhotonNetwork.CurrentRoom;
        if (readyPlayers == (int)room.CustomProperties["playerCount"]){
            return true;
        }
        return false;
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
                    listing.name = player.UserId;
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
                    listing.name = player.UserId;
                    PlayerListingInfo playerInfo = listing.GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, true);
                    cachedPlayerList[player.UserId] = listing;
                }
            }
        }
    }

    bool AddToPlayerCount(int amount){ //this should use CAS and hence be network-safe BUT IT ISN'T :(((((
            Room room = PhotonNetwork.CurrentRoom;
            int playerCount = (int)room.CustomProperties["playerCount"];
            Debug.Log("Original count: " + playerCount);
            ExitGames.Client.Photon.Hashtable expectedVals = new ExitGames.Client.Photon.Hashtable();
            expectedVals.Add("playerCount", playerCount);
            int newPlayerCount  = playerCount  + amount;
            ExitGames.Client.Photon.Hashtable newVals = new ExitGames.Client.Photon.Hashtable();
            newVals.Add("playerCount", newPlayerCount);
            //room.SetCustomProperties(newVals);
            if (!room.SetCustomProperties(newVals, expectedVals)){
                return false;
            } else {
                //debug
                StartCoroutine(Printer(0.2f));
                IEnumerator Printer(float delay){
                    yield return new WaitForSeconds(delay);
                    Room newRoom = PhotonNetwork.CurrentRoom;
                    Debug.Log("New count: " + (int)newRoom.CustomProperties["playerCount"]);
                }
                return true;
            }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddToPlayerCount(1);
        if (cachedPlayerList.Count < 2){
            GameObject listing =  Instantiate(_playerListing,_content);
            listing.name = newPlayer.UserId;
            PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
            roomInfo.SetPlayerInfo(newPlayer, false);
            cachedPlayerList[newPlayer.UserId] = listing;
        } else {
            GameObject listing =  Instantiate(_playerListing,_specContent);
            listing.name = newPlayer.UserId;
            PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
            roomInfo.SetPlayerInfo(newPlayer, true);
            cachedPlayerList[newPlayer.UserId] = listing;
        }
    }
    [PunRPC]
    void UpdateReadyListingsRPC(string UserID, bool isReady){ //updates text colour
        PlayerListingInfo listing = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>();
        if (isReady){
            _content.Find(listing.name).GetComponentInChildren<Text>().color = readyColour;
        } else {
            _content.Find(listing.name).GetComponentInChildren<Text>().color = unreadyColour;
        }
    }

    void UpdateReadyListings(string UserID, bool isReady){ //updates text colour
        pv.RPC("UpdateReadyListingsRPC", RpcTarget.All, UserID, isReady);
        // PlayerListingInfo listing = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>();
        // if (isReady){
        //     _content.Find(listing.name).GetComponentInChildren<Text>().color = readyColour;
        // } else {
        //     _content.Find(listing.name).GetComponentInChildren<Text>().color = unreadyColour;
        // }
    }

    [PunRPC]
    void UpdateSpectatorListingsRPC(string UserID, bool isSpectator){ //updates text colour
        PlayerListingInfo listing;
        if (isSpectator){
             listing = cachedSpectatorList[UserID].GetComponent<PlayerListingInfo>();
            _specContent.Find(listing.name).GetComponentInChildren<Text>().color = specColour;
        } else {
            listing = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>();
            _content.Find(listing.name).GetComponentInChildren<Text>().color = unreadyColour;
        }
    }

    void UpdateSpectatorListings(string UserID, bool isSpectator){ //updates text colour
        // PlayerListingInfo listing;
        // if (isSpectator){
        //      listing = cachedSpectatorList[UserID].GetComponent<PlayerListingInfo>();
        //     _specContent.Find(listing.name).GetComponentInChildren<Text>().color = specColour;
        // } else {
        //     listing = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>();
        //     _content.Find(listing.name).GetComponentInChildren<Text>().color = unreadyColour;
        // }
        pv.RPC("UpdateSpectatorListingsRPC", RpcTarget.All, UserID, isSpectator);
    }

    [PunRPC] 
    public void ToggleReadyRPC(string UserID, bool toReady){
        if (toReady){
            readyPlayers++;
        } else{
            readyPlayers--;
        }
        UpdateReadyListings(UserID, toReady);
    }

    public void ToggleReady(){
        if (readyCooldown <= 0){
            readyCooldown = readyCooldownMax;
            Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;
            Debug.Log(player.UserId);
            if (cachedPlayerList.ContainsKey(player.UserId)){
                PlayerListingInfo listing = cachedPlayerList[player.UserId].GetComponent<PlayerListingInfo>();
                if (!listing._spectator){
                    listing.isReady = !listing.isReady;
                    ToggleReadyRPC(player.UserId, listing.isReady);
                }
            } else{
                Debug.Log("Attempting to 'Ready' as spectator.");
            }
        }
    }

    [PunRPC] 
    public void SwapSpectateStateRPC(string UserID, bool toSpectator){
        Room room = PhotonNetwork.CurrentRoom;
        Photon.Realtime.Player player = null;
        foreach (Photon.Realtime.Player p in room.Players.Values){ //I hate this but photon doesn't leave me much choice as far as I can tell
            if (p.UserId == UserID){
                player = p;
            }
        }
        if (player != null){
            if (toSpectator)
            {
                if (cachedPlayerList.ContainsKey(UserID))
                {
                    if (cachedPlayerList[UserID].GetComponent<PlayerListingInfo>().isReady){
                        readyPlayers--;
                    }
                    Destroy(cachedPlayerList[UserID]);
                    cachedPlayerList.Remove(player.UserId);
                    GameObject listing = Instantiate(_playerListing, _specContent);
                    listing.name = player.UserId;
                    PlayerListingInfo playerInfo = listing.GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, true);
                    cachedSpectatorList[player.UserId] = listing;
                }
            }
            else
            {
                if (cachedSpectatorList.ContainsKey(UserID))
                {
                    Destroy(cachedSpectatorList[player.UserId]);
                    cachedSpectatorList.Remove(player.UserId);
                    GameObject listing = Instantiate(_playerListing, _content);
                    listing.name = player.UserId;
                    PlayerListingInfo playerInfo = listing.GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, false);
                    cachedPlayerList[player.UserId] = listing;
                }
            }
            UpdateSpectatorListings(UserID, toSpectator);
        }
    }

    public void SwapSpectateState(){
        if (spectateCooldown <= 0){
            spectateCooldown = spectateCooldownMax;
            if (pv == null) return;
            Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;
            Room room = PhotonNetwork.CurrentRoom;
            int maxSpectators = room.MaxPlayers - 2;
            if (cachedSpectatorList.Count < maxSpectators && cachedPlayerList.ContainsKey(player.UserId)){
                if (AddToPlayerCount(-1)){
                pv.RPC("SwapSpectateStateRPC", RpcTarget.All, player.UserId, true);
                spectateButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player";
                } else {
                    Debug.Log("Error: another player is currently attempting to swap. [PLACEHOLDER]");
                }
            } else if (cachedPlayerList.Count < 2 && cachedSpectatorList.ContainsKey(player.UserId)){
                if (AddToPlayerCount(1)){
                    pv.RPC("SwapSpectateStateRPC", RpcTarget.All, player.UserId, false);
                    spectateButton.GetComponentInChildren<TextMeshProUGUI>().text = "Spectator";
                } else {
                    Debug.Log("Error: another player is currently attempting to swap. [PLACEHOLDER]");
                }
                
            }
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

    private IEnumerator SyncedLobbyTimers() {
        while (true) {
            if (readyCooldown > 0) {
                readyCooldown -= Time.deltaTime;
            }
            if (spectateCooldown > 0) {
                spectateCooldown -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
