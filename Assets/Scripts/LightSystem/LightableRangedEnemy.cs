using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.WeaponSystem;

namespace LightsOn.LightingSystem {

    public class LightableRangedEnemy : LightableEnemy {

        public SkinnedMeshRenderer smr;
        public EnemyGun gunScript;
        public LineRenderer lineRenderer;
        public ColouredMaterial lineMaterials;
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
                lineRenderer.material = lineMaterials.get(colour);
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