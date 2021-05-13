using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    public GameObject MicToggle;
    public GameObject VoiceChatToggle;
    public GameObject VoiceControlToggle;
    // Start is called before the first frame update
    void Start()
    {
        if(!GlobalValues.micEditable) 
        {
            MicToggle.interactable = false;
            VoiceChatToggle.interactable = false;
            VoiceControlToggle.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateMicEnabled(bool newVal)
    {

    }
    public void updateVoiceChatEnabled(bool newVal)
    {
        
    }
    public void updateVoiceControlEnabled(bool newVal)
    {
        
    }
}
