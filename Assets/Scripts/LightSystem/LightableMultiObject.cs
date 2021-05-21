using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightsOn.LightingSystem {

    public class LightableMultiObject : LightableObject {
        public List<Renderer> childObjects = null;

        //overrided method to set all child object colour
        public override void SetColour(LightColour col) {
            base.SetColour(col);
            if (initialised) {
                if (childObjects != null) {
                    foreach (Renderer r in childObjects) {
                        r.material = materials.get(colour);
                    }
                }
            }
        }

        //overrided method to make all child objects visually disappear 
        public override void Disappear() {
            base.Disappear();
            if (childObjects != null) {
                foreach (Renderer mr in childObjects) {
                    mr.material = hiddenMaterials.get(colour);
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
        }

        //overrided method to make all child objects visually appear
        public override void Appear() {
            base.Appear();
            if (childObjects != null) {
                foreach (Renderer mr in childObjects) {
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    mr.material = materials.get(colour);
                }
            }
        }
    }
}