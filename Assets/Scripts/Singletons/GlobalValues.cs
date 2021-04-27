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
    
    public static GlobalValues Instance { get { return _instance; } }

    public void AddPlayer(GameObject player) {
        if (!players.Contains(player)) {
            players.Add(player);
            fm.SetPlayerNum(players.Count);
        }
    }
    public void updateMicPermissions(bool micEnabledValue) {
        micEnabled = micEnabledValue;
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
        _instance.respawnPoint = respawnPoint;
        _instance.p1spawn = p1spawn;
        _instance.p2Spawn = p2Spawn;
        _instance.fm = fm;
        _instance.navManager = navManager;
        gameObject.GetComponent<LocalObjectPool>().RespawnBoids();
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            UpdateGlobalValues();
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
            GameObject UI = Instantiate(UIPrefab);
            DontDestroyOnLoad(UI);
            _instance.UIElements = UI;
        }
    }
}