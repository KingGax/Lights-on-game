using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NavigationPoint : MonoBehaviour
{
    public NavigationManager navigationManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("TRIGGER!");
        if (((1 << other.gameObject.layer) & GlobalValues.Instance.playerLayer) != 0){
            Debug.Log("Yo");
            //PhotonView pv = other.gameObject.GetComponent<PhotonView>();
            if (GlobalValues.Instance.localPlayerInstance.transform.position == other.transform.position){
                navigationManager.UpdateManager(transform.position);
            }
        }
        
    }
}
