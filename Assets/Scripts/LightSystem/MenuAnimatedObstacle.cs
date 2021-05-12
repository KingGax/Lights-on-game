using UnityEngine;

namespace LightsOn.LightingSystem {
    public class MenuAnimatedObstacle : LightableObstacle {

        protected void Start() {
            base.Start();
            Disappear();
        }

        public override void Appear() {
            base.Appear();
            Invoke("Disappear", 5.0f);
        }

        public override void Disappear() {
            base.Disappear();
            Invoke("Appear", 5.0f);
        }
    }
}