using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using static System.DateTime;
using static System.TimeSpan;

public class TimerCountdownText : MonoBehaviour
{

    private TextMeshProUGUI text;
    private System.DateTime startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = System.DateTime.Now;
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        System.TimeSpan timedif = System.DateTime.Now - startTime; 
        text.SetText(timedif.ToString(@"mm\:ss\:fff"));
    }
}
