using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostprocessorSingleton : MonoBehaviour
{

    public static PostprocessorSingleton Instance { get { return _instance; } }
    private static PostprocessorSingleton _instance;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}
