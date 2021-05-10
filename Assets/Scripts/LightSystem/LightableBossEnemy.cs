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
            boidInstance = GetCurrentBoidManagerInstance();
            boidInstance.SetFollowTransform(transform.parent);
            boidInstance.agentSpeed = boidSpeed;
        }

        public void BossReappear() {
            stayHidden = false;
        }

        public override void ForceDisappear() {
            stayHidden = true;
            base.ForceDisappear();
        }

        protected override void TryAppear() {
            if (!stayHidden) {
                base.TryAppear();
            }
        }

    }
}

