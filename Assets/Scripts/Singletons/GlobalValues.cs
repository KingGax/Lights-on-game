using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GlobalValues : MonoBehaviour {
    private static GlobalValues _instance;
    public LayerMask reappearPreventionLayers;
    public LayerMask environment;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public LayerMask playerOrHiddenPlayerMask;
    public LayerMask shootTargetsLayer;
    public List<GameObject> players;
    public GameObject localPlayerInstance;
    public GameObject UIElements;
    public GameObject UIPrefab;
    public GameObject boidManagerPrefab;
    public GameObject boidDeathPrefab;
    public GameObject MenuItem;
    public Material defaultGreen;
    public Material defaultBlue;
    public Material defaultRed;
    public Transform respawnPoint;
    public Transform p1spawn;
    public Transform p2Spawn;
    public FloorManager fm;
    public NavigationManager navManager;
    public bool micEnabled = true;
    public bool micEditable = true;
    public bool voiceChatEnabled = true;
    public bool voiceControlEnabled = true;
    public bool enableShader = true;
    public bool enableBoids = true;
    public bool bothPlayersSpawned = false;
    public bool p1Spawned = false;
    public bool p2Spawned = false;
    public bool hasSeenMainMenu = false;

    public static GlobalValues Instance { get { return _instance; } }

    public void AddPlayer(GameObject player) {
        if (!players.Contains(player)) {
            players.Add(player);
            fm.SetPlayerNum(players.Count);
        }
    }
    public void updateMicPermissions(bool micEnabledValue) {
        this.micEnabled = micEnabledValue;
        if(micEnabledValue == false) {
            this.voiceChatEnabled = false;
            this.voiceControlEnabled = false;
        }
    }
    public void disableMicFrontend() {
        updateMicPermissions(false);
        PlayerController pc = GetComponent<PlayerController>();
        pc.micRenderer.enabled = false;
        pc.ChangeLight();
        micEditable = false;
    }

    public void PlayerLeft()
    {
        /*for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null)
            {
                if (i == 0)
                {
                    players[i] = players[i + 1];
                }
                else
                {
                    players[i] = players[i - 1];
                }
            }
        }*/
    }

    private void UpdateGlobalValues() {
        _instance.UIPrefab = UIPrefab;
        _instance.boidManagerPrefab = boidManagerPrefab;
        _instance.boidDeathPrefab = boidDeathPrefab;
        _instance.defaultRed = defaultRed;
        _instance.defaultGreen = defaultGreen;
        _instance.defaultBlue = defaultBlue;
        _instance.respawnPoint = respawnPoint;
        _instance.p1spawn = p1spawn;
        _instance.p2Spawn = p2Spawn;
        _instance.fm = fm;
        _instance.navManager = navManager;
        _instance.boidManagerPrefab = boidManagerPrefab;
        _instance.enableBoids = enableBoids;
        _instance.enableShader = enableShader;
        _instance.bothPlayersSpawned = bothPlayersSpawned; 

        gameObject.GetComponent<LocalObjectPool>().RespawnBoids();

        _instance.GetComponent<ShaderPlayerTracker>().enabled = enableShader;
        _instance.GetComponent<LocalObjectPool>().enabled = enableBoids;
    }

    private void UpdateUI() {
        if (UIPrefab != null && _instance.UIElements == null) {
            GameObject UI = Instantiate(UIPrefab);
            DontDestroyOnLoad(UI);
            _instance.UIElements = UI;
        }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            UpdateGlobalValues();
            UpdateUI();
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
            _instance.GetComponent<ShaderPlayerTracker>().enabled = enableShader;
            _instance.GetComponent<LocalObjectPool>().enabled = enableBoids;
            UpdateUI();
        }
    }
}