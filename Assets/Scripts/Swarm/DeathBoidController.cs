using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LightsOn.LightingSystem {
    public class DeathBoidController : MonoBehaviour {
        BoidManager parent;
        public float riseSpeed;
        public float disperseSpeed;
        //public float boidSpeed;
        //public float turnSpeed;
        public float effectTime;
        // Start is called before the first frame update
        void Start() {
            parent = GetComponentInParent<BoidManager>();
            parent.DirectAwayFromCentre(disperseSpeed);
            parent.StartRising(riseSpeed);
            //parent.SetSpeeds(boidSpeed, turnSpeed);
            Invoke("DestroyBoids", effectTime);
        }

        void DestroyBoids() {
            parent.DestroyMyAgents();
        }

        // Update is called once per frame
        void Update() {
            //parent.MoveBoidCentre(new Vector3(parent.boidCentre.x, parent.boidCentre.y + riseSpeed * Time.deltaTime, parent.boidCentre.z));
        }
    }
}

