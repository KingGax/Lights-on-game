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
    //bool playerSet = false;
    int navIndex = 0; //index of target point
    // Start is called before the first frame update
    void Start()
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
    protected void SetPointsRPC(){
        if (!isMaster){
            navigationPoints = floorManager.p1NavPoints;
        } else {
            navigationPoints = floorManager.p2NavPoints;
        }
    }

    public void SetPlayer(bool master){
        playerSet = true;
        isMaster = master;
        if (started){
            SetPoints();
            arrow.UpdateTarget(navigationPoints[navIndex].transform);
            arrow.SetEnabled(navigationEnabled);
        }
    }

    public void SetPoints(){
        //initialised = true;
        arrow.enabled = true;
        if (isMaster){
            navigationPoints = floorManager.p1NavPoints;
        } else {
            navigationPoints = floorManager.p2NavPoints;
        }
        pv.RPC("SetPointsRPC", RpcTarget.All);
        //navigationPoints = points;
    }

    public void UpdateManager(Vector3 gate){
        if (navigationPoints[navIndex].transform.position == gate){
            if (navIndex < navigationPoints.Count-1){
                navIndex++;
            } else {
                arrow.enabled = false;
                return;
            }
        } else { //backtracking
            //navIndex--; //do nothing for now, to prevent entering and leaving from the same side, etc.
        }
        //Debug.Log("Point: " + navigationPoints[navIndex].transform.position);
        arrow.UpdateTarget(navigationPoints[navIndex].transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 rot = arrow.transform.rotation.eulerAngles;
        
    }
}
