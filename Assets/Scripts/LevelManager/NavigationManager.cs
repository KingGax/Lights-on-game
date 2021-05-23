using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NavigationManager : MonoBehaviour
{
    bool playerSet = false;
    public List<NavigationPoint> navigationPoints;
    NavArrowController arrow;
    FloorManager floorManager;
    public bool navigationEnabled = false;
    PhotonView pv;
    bool isMaster = true;
    bool started = false;
    int navIndex = 0; //index of target point
    // Start is called before the first frame update
    void Start() //Setup components, enable compass UI if player assigned to manager
    {
        pv = gameObject.GetPhotonView();
        navigationPoints = new List<NavigationPoint>();
        arrow = GlobalValues.Instance.UIElements.GetComponentInChildren<NavArrowController>();
        
        floorManager = GlobalValues.Instance.fm;
        if (playerSet){
            SetPoints();
            arrow.UpdateTarget(navigationPoints[navIndex].transform);
            arrow.SetEnabled(navigationEnabled);
        }
        started = true;
    }

    [PunRPC]
    protected void SetPointsRPC(){ //RPC for assigning navpoints to manager
        if (!isMaster){
            navigationPoints = floorManager.p1NavPoints;
            navIndex = floorManager.GetPlayerRoom(true);
        } else {
            navigationPoints = floorManager.p2NavPoints;
            navIndex = floorManager.GetPlayerRoom(false);
        }
    }

    public void SetPlayer(bool master){ //Assign manager to player
        playerSet = true;
        isMaster = master;
        if (started){
            SetPoints();
            arrow.UpdateTarget(navigationPoints[navIndex].transform);
            arrow.SetEnabled(navigationEnabled);
        }
    }

    public void SetPoints(){ //Set points according to player index
        arrow.enabled = true;
        if (isMaster){
            navigationPoints = floorManager.p1NavPoints;
        } else {
            navigationPoints = floorManager.p2NavPoints;
        }
        pv.RPC("SetPointsRPC", RpcTarget.All);
    }

    public void UpdateManager(Vector3 gate){ //Update next navpoint
        if (navigationPoints[navIndex].transform.position == gate){
            if (navIndex < navigationPoints.Count-1){
                navIndex++;
            } else {
                arrow.enabled = false;
                return;
            }
        } else {
        }
        arrow.UpdateTarget(navigationPoints[navIndex].transform);
        
    }
}
