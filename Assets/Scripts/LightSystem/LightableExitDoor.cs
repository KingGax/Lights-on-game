using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using LightsOn.AudioSystem;

namespace LightsOn.LightingSystem {

    public class LightableExitDoor : LightableObject {
        public Light light;
        public LightColour unlockedColour;
        public PointCloudSO ballCloud;
        bool disappeared = false;

        public override void Start() {
            base.Start();

        }
        public void LockDoor() {
            disappeared = false;
            transform.parent.gameObject.layer = defaultLayer;
            SetColour(LightColour.White);
        }

        [PunRPC]
        public void PuzzleBallUnlockDoorRPC(LightColour ballCol, Vector3 ballPos) {
            Vector3[] doorPoints = GetTransformedPoints();
            Vector3[] ballPoints = ballCloud.points.ToArray();
            for (int i = 0; i < ballPoints.Length; i++) {
                ballPoints[i] += ballPos;
            }
            SpawnDeathCloud(ballPoints.Concat(doorPoints).ToArray(), ballCol);
            BoidManager bm = GetCurrentBoidManagerInstance();
            bm.MoveBoidCentre(ballPos);
            unlockedColour = colour.Subtract(ballCol);
        }

        public void PuzzleBallUnlockDoor(LightColour ballCol, Vector3 ballPos) {
            Vector3[] doorPoints = GetTransformedPoints();
            Vector3[] ballPoints = ballCloud.points.ToArray();
            for (int i = 0; i < ballPoints.Length; i++) {
                ballPoints[i] += ballPos;
            }
            SpawnDeathCloud(ballPoints.Concat(doorPoints).ToArray(), ballCol);
            BoidManager bm = GetCurrentBoidManagerInstance();
            bm.MoveBoidCentre(ballPos);
            unlockedColour = colour.Subtract(ballCol);
            UnlockDoor();
        }

        public override void SetColour(LightColour col) {
            base.SetColour(col);
            light.color = colour.DoorLightColour();
        }

        public void UnlockDoor() {
            colour = unlockedColour;
            SetColour(unlockedColour);
            AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXDoorOpen, transform.position, gameObject);
        }

        public override void Appear() {
            if (!disappeared) {
                base.Appear();
            }
        }

        public override void Disappear() {
            if (!disappeared) {
                base.Disappear();
                disappeared = true;
            }
        }
    }
}