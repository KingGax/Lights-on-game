using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GlobalValues : MonoBehaviour
{
    private static GlobalValues _instance;
    public LayerMask reappearPreventionLayers;
    public LayerMask environment;
    public LayerMask playerLayer;
    public List<GameObject> players;
    public Material hiddenGreen;
    public Material hiddenBlue;
    public Material hiddenRed;
    public Material hiddenCyan;
    public Material hiddenYellow;
    public Material hiddenMagenta;
    public static GlobalValues Instance { get { return _instance; } }

    public void AddPlayer(GameObject player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }

    }
}