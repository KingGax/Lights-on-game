using UnityEngine;

namespace LightsOn.AudioSystem {
    public class SoundClips : MonoBehaviour {
        
        private static SoundClips _instance;
        public static SoundClips Instance { get { return _instance; } }

        public SFX SFXLazer;
        public SFX SFXShoot;
        public SFX SFXDoorOpen;
        public SFX SFXLightChange;
        public SFX SFXKill;
        public SFX SFXDash;
        public SFX SFXMenuClicks;
        public SFX SFXDisappear;
        public SFX SFXBallBounce;
        public SFX SFXBallShatter;
        public SFX SFXEnemyExplosion;
        public SFX SFXEnemyGunfire;
        public SFX SFXPlayerHit;

        public void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
                _instance = this;
            }
        }
    }
}