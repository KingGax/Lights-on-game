using UnityEngine;
using System.Runtime.InteropServices;

namespace LightsOn {
namespace JSBridge {

public class JSVoice : MonoBehaviour {

    private static JSVoice _instance;
    private bool enabled = false;

    [DllImport("__Internal")]
    private static extern void startRecogniser();

    [DllImport("__Internal")]
    private static extern void isVoiceLoadedUnityCallback();

    [DllImport("__Internal")]
    private static extern void setUpVoiceDetection();

    public void voiceLoadedCallback(bool loaded) {
        if (loaded) {
            setUpVoiceDetection();
        }
    }

    public void micEnabledCallback(bool enabled) {
        this.enabled = enabled;
    }

    public void requestVoiceActivation() {
        isVoiceLoadedUnityCallback();
    }
     
    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}}}