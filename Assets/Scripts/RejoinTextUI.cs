using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RejoinTextUI : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void DisplayRejoinText(bool show) {
        text.enabled = show;
    }

    public void SetRejoinText(string newText) {
        text.text = newText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
