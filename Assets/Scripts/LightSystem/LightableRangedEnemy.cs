using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.WeaponSystem;

namespace LightsOn.LightingSystem {

    public class LightableRangedEnemy : LightableEnemy {

        public SkinnedMeshRenderer smr;
        public EnemyGun gunScript;
        public LineRenderer lineRenderer;
        private Animator anim;
        //allows for non-modelled ranged enemies, remove once sniper has a model

        public override void Start() {
            anim = transform.parent.GetComponent<Animator>();
            if (anim == null) {
                anim = transform.parent.GetComponentInChildren<Animator>();
            }
            base.Start();
        }

        public override void SetColour(LightColour col) {
            base.SetColour(col);
            if (gunScript != null) {
                gunScript.SetColour(colour);
            }
            if (initialised) {
                smr.material = materials.get(colour);
            }
            if (lineRenderer != null) {
                Material rendererMat = GlobalValues.Instance.defaultRed;
                switch (col) {
                    case LightColour.Black:
                        break;
                    case LightColour.Red:
                        rendererMat = GlobalValues.Instance.defaultRed;
                        break;
                    case LightColour.Green:
                        rendererMat = GlobalValues.Instance.defaultGreen;
                        break;
                    case LightColour.Blue:
                        rendererMat = GlobalValues.Instance.defaultBlue;
                        break;
                    case LightColour.Cyan:
                        break;
                    case LightColour.Magenta:
                        break;
                    case LightColour.Yellow:
                        break;
                    case LightColour.White:
                        break;
                    default:
                        break;
                }
                lineRenderer.material = rendererMat;
            }
        }

        public override void Disappear() {
            anim.speed = 0;
            smr.material = hiddenMaterials.get(colour);
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            base.Disappear();
        }

        public override void Appear() {
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            smr.material = materials.get(colour);
            base.Appear();
        }

        protected override void LerpMaterial(float lerp) {
            base.LerpMaterial(lerp);
            if (overrideMeshRenderer) {
                smr.material.Lerp(hiddenMaterials.get(colour), materials.get(colour), lerp);
            }
        }



        public override void FinishAppearing() {
            anim.speed = 1;
            if (overrideMeshRenderer) {
                smr.material = materials.get(colour);
            }
            base.FinishAppearing();

        }

    }
}