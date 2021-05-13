using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject MenuItem;
    public Toggle MicToggle;
    public Toggle VoiceChatToggle;
    public Toggle VoiceControlToggle;
    bool visible = false;
    // Start is called before the first frame update
    void Start()
    {
        GlobalValues.Instance.MenuItem = MenuItem;
        MenuItem.SetActive(false);
        if(GlobalValues.Instance.micEditable == false) 
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
    public bool ToggleVisibility() {
        MenuItem.SetActive(!visible);
        visible = !visible;
        return visible;
    }
    public void updateMicEnabled(bool newVal)
    {
        GlobalValues.Instance.micEnabled = newVal;
    }
    public void updateVoiceChatEnabled(bool newVal)
    {
        GlobalValues.Instance.voiceChatEnabled = newVal;
    }
    public void updateVoiceControlEnabled(bool newVal)
    {
        GlobalValues.Instance.voiceControlEnabled = newVal;
    }
}
