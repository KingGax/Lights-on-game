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
    public GameObject enemyParent;
    public GameObject enemyContainers;
    public List<GameObject> players;
    public GameObject localPlayerInstance;
    public GameObject UIElements; 
    public Material defaultGreen;
    public Material defaultBlue;
    public Material defaultRed;
    public Material hiddenGreen;
    public Material hiddenBlue;
    public Material hiddenRed;
    public Material hiddenCyan;
    public Material hiddenYellow;
    public Material hiddenMagenta;
    public Transform respawnPoint;
    public Transform p1spawn;
    public Transform p2Spawn;
    public FloorManager fm;
    
    public static GlobalValues Instance { get { return _instance; } }

    public void AddPlayer(GameObject player) {
        if (!players.Contains(player)) {
            players.Add(player);
            fm.SetPlayerNum(players.Count);
        }
    }

    public void PlayerLeft()
    {
        for (int i = 0; i < players.Count; i++)
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
        }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}