using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class Lobby : MonoBehaviour
{
    [SerializeField]
    private GameObject startButton;

    void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }
    public void StartGame() {
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 2);
    }
}
