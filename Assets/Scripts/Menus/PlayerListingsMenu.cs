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
    public PlayerListingText p1;
    public PlayerListingText p2;

    public Color readyColour = Color.green;
    public Color unreadyColour = Color.red;
    public Color specColour = Color.black;
    [SerializeField]
    public GameObject _playerListing;
    PhotonView pv;
    int readyPlayers = 0;
    public Button spectateButton;

    bool initialised = false;

    public Dictionary<string, GameObject> cachedPlayerList = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> cachedSpectatorList = new Dictionary<string, GameObject>();

    public float readyCooldownMax;
    float readyCooldown;
    public float spectateCooldownMax;
    float spectateCooldown;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        StartCoroutine("SyncedLobbyTimers");
        GameObject temp = GameObject.Find("P1");
        if (temp != null) {
            p1 = temp.GetComponent<PlayerListingText>();
            p2 = GameObject.Find("P2").GetComponent<PlayerListingText>();
            if (PhotonNetwork.IsMasterClient) {
                cachedPlayerList.Clear();
                initialised = true;
            } else {
                GameObject lobby = GameObject.Find("Lobby");
                if (lobby != null) {
                    transform.SetParent(lobby.transform);
                    transform.position = new Vector3(lobby.transform.position.x - 10, lobby.transform.position.y, lobby.transform.position.z);
                    lobby.GetComponent<Lobby>().SetReadyButton();
                }
            }
        }
    }

    public override void OnEnable() //On script enabled, get all players in room, update listings
    {
        base.OnEnable();
        GetCurrentPlayers();
    }

    public bool isReady(){ //Check if all players are ready
        Room room = PhotonNetwork.CurrentRoom;
        if (readyPlayers == (int)room.CustomProperties["playerCount"]){
            return true;
        }
        return false;
    }

    public override void OnDisable() //Destroy all listings, clear list of players when script disabled
    {
        base.OnDisable();
        foreach(KeyValuePair<string, GameObject> elem in cachedPlayerList) {
            Destroy(elem.Value);
        }
        cachedPlayerList.Clear();
    }

    private void GetCurrentPlayers() //Updates listings for players and spectators in room
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        int counter = 0;
        foreach(KeyValuePair<int, Player> elem in PhotonNetwork.CurrentRoom.Players) {
            
            Player player = elem.Value;
            if (counter < Mathf.Min(PhotonNetwork.CurrentRoom.Players.Count, 2)){ 
                PlayerListingInfo playerInfo;
                if (cachedPlayerList.ContainsKey(player.UserId)){
                    playerInfo = cachedPlayerList[player.UserId].GetComponent<PlayerListingInfo>();
                }
                else {
                    GameObject listing =  Instantiate(_playerListing,_content);
                    listing.name = player.UserId;
                    playerInfo = listing.GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, false);
                    cachedPlayerList[player.UserId] = listing;
                }
                UpdateReadyListings(player.UserId, playerInfo.isReady);
            } else {
                PlayerListingInfo playerInfo;
                if (cachedSpectatorList.ContainsKey(player.UserId)){
                    playerInfo = cachedPlayerList[player.UserId].GetComponent<PlayerListingInfo>();
                }
                else {
                    GameObject listing =  Instantiate(_playerListing,_specContent);
                    listing.name = player.UserId;
                    playerInfo = listing.GetComponent<PlayerListingInfo>();
                    playerInfo.SetPlayerInfo(player, true);
                    cachedPlayerList[player.UserId] = listing;
                }
                UpdateSpectatorListings(player.UserId, playerInfo._spectator);
            }
            counter++;
        }
    }


    //Returns true on success, false on failure
    bool SetActorNumber(int actorNumber) { //CAS set room actor number
        Room room = PhotonNetwork.CurrentRoom;
        int oldHighestActor = (int)room.CustomProperties["highestActor"];
        ExitGames.Client.Photon.Hashtable expectedVals = new ExitGames.Client.Photon.Hashtable();
        expectedVals.Add("highestActor", oldHighestActor);
        ExitGames.Client.Photon.Hashtable newVals = new ExitGames.Client.Photon.Hashtable();
        newVals.Add("highestActor", actorNumber);
        if (!room.SetCustomProperties(newVals, expectedVals)) {
            return false;
        } else {
            return true;
        }
    }

    //Returns true on success, false on failure
    bool AddToPlayerCount(int amount){ //CAS addition to room's player count
            Room room = PhotonNetwork.CurrentRoom;
            int playerCount = (int)room.CustomProperties["playerCount"];
            ExitGames.Client.Photon.Hashtable expectedVals = new ExitGames.Client.Photon.Hashtable();
            expectedVals.Add("playerCount", playerCount);
            int newPlayerCount  = playerCount  + amount;
            ExitGames.Client.Photon.Hashtable newVals = new ExitGames.Client.Photon.Hashtable();
            newVals.Add("playerCount", newPlayerCount);
            if (!room.SetCustomProperties(newVals, expectedVals)){
                return false;
            } else {
                return true;
            }
    }

    [PunRPC] 
    public void SetTruthValuesRPC(string UserID, bool isSpectator, bool isReady){ //RPC for updating user's isReady and isSpectator states
        if (!pv.IsMine && initialised == false){
            initialised = true;
            PlayerListingInfo listingInfo;
            if (isSpectator){
                listingInfo = cachedSpectatorList[UserID].GetComponent<PlayerListingInfo>();
                listingInfo._spectator = isSpectator;
                listingInfo.isReady = isReady;
                
            } else {
                listingInfo = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>();
                listingInfo._spectator = isSpectator;
                listingInfo.isReady = isReady;
                
            }
            UpdateSpectatorListings(UserID, listingInfo._spectator);
            UpdateReadyListings(UserID, listingInfo.isReady);
        }
    }

    public void SetTruthValues(){ //Update player listing's isReady and isSpectator values
        if (cachedPlayerList.Count == 0 && cachedSpectatorList.Count == 0){
            GetCurrentPlayers();
        }
        foreach(KeyValuePair<int, Player> elem in PhotonNetwork.CurrentRoom.Players) {
            PlayerListingInfo listingInfo;
            if (cachedPlayerList.ContainsKey(elem.Value.UserId)){
                listingInfo = cachedPlayerList[elem.Value.UserId].GetComponent<PlayerListingInfo>();
                pv.RPC("SetTruthValuesRPC", RpcTarget.All, elem.Value.UserId, listingInfo._spectator, listingInfo.isReady);
            } else if (cachedSpectatorList.ContainsKey(elem.Value.UserId)){
                listingInfo = cachedSpectatorList[elem.Value.UserId].GetComponent<PlayerListingInfo>();
                pv.RPC("SetTruthValuesRPC", RpcTarget.All, elem.Value.UserId, listingInfo._spectator, listingInfo.isReady);
            }
            
        } 
    }

    //Called when player enters room. Checks if new player should be a spectator or not and calls appropriate functions
    public override void OnPlayerEnteredRoom(Player newPlayer) 
    {
        if (p1 != null) {
            if (cachedPlayerList.Count < 2) {
                AddToPlayerCount(1);
                GameObject listing = Instantiate(_playerListing, _content);
                listing.name = newPlayer.UserId;
                PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
                roomInfo.SetPlayerInfo(newPlayer, false);
                cachedPlayerList[newPlayer.UserId] = listing;
                UpdateReadyListings(newPlayer.UserId, false);
            } else {
                GameObject listing = Instantiate(_playerListing, _specContent);
                listing.name = newPlayer.UserId;
                PlayerListingInfo roomInfo = listing.GetComponent<PlayerListingInfo>();
                roomInfo.SetPlayerInfo(newPlayer, true);
                cachedPlayerList[newPlayer.UserId] = listing;
            }
            SetTruthValues();
        }
    }

    void UpdateReadyListings(string UserID, bool isReady){ //updates text colour of ready listing
        if (p1 != null) {
            PlayerListingInfo listing = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>();
            string thisPlayerName = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>().playerName;
            if (p1.userID == UserID) {
                p1.text.color = isReady ? readyColour : unreadyColour;
                p1.light.enabled = true;
            } else if (p2.userID == UserID) {
                p2.text.color = isReady ? readyColour : unreadyColour;
                p2.light.enabled = true;
            } else if (p1.userID == "") {
                p1.text.color = isReady ? readyColour : unreadyColour;
                p1.userID = UserID;
                p1.text.text = thisPlayerName;
                p1.light.enabled = true;
            } else {
                p2.text.color = isReady ? readyColour : unreadyColour;
                p2.userID = UserID;
                p2.text.text = thisPlayerName;
                p2.light.enabled = true;
            }
        }
    }

    void UpdateSpectatorListings(string UserID, bool isSpectator){ //updates text colour for spectator listings (unused atm)
        PlayerListingInfo listing;
        if (isSpectator){
             listing = cachedSpectatorList[UserID].GetComponent<PlayerListingInfo>();
            _specContent.Find(listing.name).GetComponentInChildren<Text>().color = specColour;
        } else {
            listing = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>();
            _content.Find(listing.name).GetComponentInChildren<Text>().color = unreadyColour;
        }
    }

    [PunRPC]
    public void AddToReadyPlayers(int val){ //RPC to alter ready player count
        readyPlayers+=val;
    }

    [PunRPC] 
    public void ToggleReadyRPC(string UserID, bool toReady){ //RPC to set user's readystate
        if (toReady){
            readyPlayers++;
        } else{
            readyPlayers--;
        }
        UpdateReadyListings(UserID, toReady);
    }

    public void ToggleReady(){ //Toggle ready state of local player
        if (readyCooldown <= 0){
            readyCooldown = readyCooldownMax;
            Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;
            if (cachedPlayerList.ContainsKey(player.UserId)){
                PlayerListingInfo listing = cachedPlayerList[player.UserId].GetComponent<PlayerListingInfo>();
                if (!listing._spectator){
                    listing.isReady = !listing.isReady;
                    pv.RPC("ToggleReadyRPC", RpcTarget.All, player.UserId, listing.isReady);
                }
            } else{
            }
        }
    }

    [PunRPC] 
    public void SwapSpectateStateRPC(string UserID, bool toSpectator){ //RPC to swap user's spectator state
        Room room = PhotonNetwork.CurrentRoom;
        
        Photon.Realtime.Player player = null;
        foreach (Photon.Realtime.Player p in room.Players.Values){ //Photon doesn't leave me much choice as far as I can tell
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
                        pv.RPC("AddToReadyPlayers", RpcTarget.All, -1);
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

    public void SwapSpectateState(){ //Swap spectator state of local player
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
                }
            } else if (cachedPlayerList.Count < 2 && cachedSpectatorList.ContainsKey(player.UserId)){
                if (AddToPlayerCount(1)){
                    pv.RPC("SwapSpectateStateRPC", RpcTarget.All, player.UserId, false);
                    spectateButton.GetComponentInChildren<TextMeshProUGUI>().text = "Spectator";
                } else {
                }
                
            }
        }
    }

    //Called when player leaves room. Removes listing and updates text
    public override void OnPlayerLeftRoom(Player otherPlayer) 
    {
        if (SceneManagerHelper.ActiveSceneName != "LobbyMenu") {
            Destroy(gameObject);
        }
        AddToPlayerCount(-1);
        if (cachedPlayerList.ContainsKey(otherPlayer.UserId)){
            string UserID = otherPlayer.UserId;
            string thisPlayerName = cachedPlayerList[UserID].GetComponent<PlayerListingInfo>().playerName;
            if (p1.userID == UserID) {
                p1.text.color = unreadyColour;
                p1.light.enabled = false;
                p1.userID = "";
                p1.text.text = "";
            } else if (p2.userID == UserID) {
                p2.text.color = unreadyColour;
                p2.light.enabled = false;
                p2.userID = "";
                p2.text.text = "";
            } 
            Destroy(cachedPlayerList[otherPlayer.UserId]);
            cachedPlayerList.Remove(otherPlayer.UserId);
        } else if (cachedSpectatorList.ContainsKey(otherPlayer.UserId)) {
            Destroy(cachedSpectatorList[otherPlayer.UserId]);
            cachedSpectatorList.Remove(otherPlayer.UserId);
        }
    }

    private IEnumerator SyncedLobbyTimers() { //timers for button cooldowns
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
