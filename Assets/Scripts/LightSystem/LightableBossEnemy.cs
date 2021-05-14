using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace LightsOn.LightingSystem {
    public class LightableBossEnemy : LightableRangedEnemy {
        private bool stayHidden = true;
        public float boidSpeed;
        BoidManager boidInstance;

        public override void Start() {
            base.Start();
        }

        public void BossSwarm() {
            ForceDisappear();
        }

        public void BossReappear() {
            stayHidden = false;
            //TryAppear();
        }

        public override void ForceDisappear() {
            stayHidden = true;
            base.ForceDisappear();
            boidInstance = GetCurrentBoidManagerInstance();
            boidInstance.SetFollowTransform(transform.parent);
        }

        protected override void TryAppear() {
            if (!stayHidden) {
                base.TryAppear();
            }
        }

    }
}

