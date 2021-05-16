using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public Toggle MicToggle;
    public Toggle VoiceChatToggle;
    public Toggle VoiceControlToggle;
    public AudioMixer mixer;

    [DllImport("__Internal")]
    private static extern void disableVoiceChatUnity();
    [DllImport("__Internal")]
    private static extern void reenableVoiceChatUnity();
    bool visible = false;
    // Start is called before the first frame update
    void Start()
    {
        MicToggle.isOn = GlobalValues.Instance.micEnabled;
        VoiceChatToggle.isOn = GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceChatEnabled;
        VoiceControlToggle.isOn = GlobalValues.Instance.micEnabled && GlobalValues.Instance.voiceControlEnabled;
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
    public void updateMicEnabled(bool newVal)
    {
        GlobalValues.Instance.micEnabled = newVal;
        if(newVal == false) {
            GlobalValues.Instance.voiceChatEnabled = false;
            GlobalValues.Instance.voiceControlEnabled = false;
            VoiceChatToggle.interactable = false;
            VoiceControlToggle.interactable = false;
            VoiceChatToggle.isOn = false;
            VoiceControlToggle.isOn = false;
            updateVoiceChatEnabled(false);
        }
        else {
            VoiceChatToggle.interactable = true;
            VoiceControlToggle.interactable = true;
        }
    }
    public void updateVoiceChatEnabled(bool newVal)
    {
        GlobalValues.Instance.voiceChatEnabled = newVal;
        if(!newVal) {
            disableVoiceChatUnity();
        }
        else {
            reenableVoiceChatUnity();
        }
    }
    public void updateVoiceControlEnabled(bool newVal)
    {
        GlobalValues.Instance.voiceControlEnabled = newVal;
    }
    public void updateVolume(float volume) {
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
    public void loadMenu() {
        SceneManager.LoadScene("StartMenu");
    }
    public void loadMenuWrapper() {
        Invoke("loadMenu", 1);
    }
}

