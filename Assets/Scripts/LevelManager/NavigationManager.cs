using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NavigationManager : MonoBehaviour
{
    bool initialised = false;
    public List<NavigationPoint> navigationPoints;
    NavArrowController arrow;
    FloorManager floorManager;
    public bool navigationEnabled = false;
    PhotonView pv;
    int navIndex = 0; //index of target point
    // Start is called before the first frame update
    void Start()
    {
        pv = gameObject.GetPhotonView();
        floorManager = GetComponent<FloorManager>();
        arrow = GlobalValues.Instance.UIElements.GetComponentInChildren<NavArrowController>();
        if (initialised){
            arrow.UpdateTarget(navigationPoints[navIndex].transform);
            arrow.SetEnabled(navigationEnabled);
        }
    }

    [PunRPC]
    protected void SetPointsRPC(bool master){
        if (master){
            navigationPoints = floorManager.p1NavPoints;
        } else {
            navigationPoints = floorManager.p2NavPoints;
        }
    }

    public void SetPoints(bool master){
        Debug.Log("Setting navpoints");
        initialised = true;
        if (master){
            navigationPoints = floorManager.p1NavPoints;
            
        } else {
            navigationPoints = floorManager.p2NavPoints;
        }
        pv.RPC("SetPointsRPC", RpcTarget.All, !master);
        //navigationPoints = points;
    }

    public void UpdateManager(Vector3 gate){
        if (navigationPoints[navIndex]){
            navIndex++;
        } else { //backtracking
            //navIndex--; //do nothing for now, to prevent entering and leaving from the same side, etc.
        }
        Debug.Log("Point: " + navigationPoints[navIndex].transform.position);
        arrow.UpdateTarget(navigationPoints[navIndex].transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 rot = arrow.transform.rotation.eulerAngles;
        
    }
}
