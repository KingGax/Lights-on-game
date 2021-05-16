using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace LightsOn.LightingSystem {
    public class LightableBossEnemy : LightableRangedEnemy {
        private bool stayHidden = true;
        public float boidSpeed;
        BoidManager boidInstance;
        public PointCloudSO cannonPoints;
        public Transform cannonTransform;

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
            SetExtraSpawnPoints(GetTransformedPoints(cannonPoints,cannonTransform));
            base.ForceDisappear();
            boidInstance = GetCurrentBoidManagerInstance();
            boidInstance.SetFollowTransform(transform.parent);
        }

        public override void Appear() {
            SetExtraSpawnPoints(GetTransformedPoints(cannonPoints, cannonTransform));
            base.Appear();
        }

        protected override void TryAppear() {
            if (!stayHidden) {
                base.TryAppear();
            }
        }


    }
}

