using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : HealthBar
{
    // Start is called before the first frame update
    Canvas hbCanvas;
    bool repositioned = false;
    float originalWidth;
    float leftPosX;
    RectTransform barRect;
    void Start()
    {
        
    }

    public override void UpdateHealth(float hp)
    {
        if (!repositioned){
            repositioned = true;
            Activate();
            
        }
        base.UpdateHealth(hp);
        //Vector3 newPos = new Vector3(0,0,0);
        // Debug.Log("bar position: "+ barRect.position.x);
        // //Debug.Log("localx: "+ barRect.transform.localScale.x);
        // //Debug.Log("original width: "+originalWidth);
        // //Debug.Log("left pos: "+ leftPosX);

        // Debug.Log("current width: "+ barRect.rect.width*barRect.transform.localScale.x);
        // newPos.x = leftPosX + (barRect.rect.width/2)*barRect.transform.localScale.x*transform.localScale.x;
        // Debug.Log("new position: " + newPos.x);
        // newPos.x = leftPosX - bar.GetComponent<Renderer>().bounds.min.x;
        // bar.Translate(newPos);
        //barRect.offsetMax = new Vector2(leftPosX, barRect.offsetMax.y);

    }

    //Called when enemy spawns, called from BossController so reparenting doesn't break anything
    //Positions healthbar and reparents it to UIElements
    public void Activate(){
        //hbCanvas = GetComponent<Canvas>();
        GameObject uielem = GlobalValues.Instance.UIElements;
        Canvas UICanvas = uielem.GetComponent<Canvas>();
        RectTransform borderSprite = GetComponentsInChildren<Image>()[0].GetComponent<RectTransform>();
        RectTransform bgSprite = GetComponentsInChildren<Image>()[1].GetComponent<RectTransform>();
        RectTransform barSprite = GetComponentsInChildren<Image>()[2].GetComponent<RectTransform>();
        transform.SetParent(uielem.transform);
        //hbCanvas.enabled = true;
        //RectTransform hbCanvasTransform = hbCanvas.GetComponent<RectTransform>();
        Vector3 pos = UICanvas.transform.position;
        pos.y = (2f*UICanvas.pixelRect.yMax + UICanvas.pixelRect.center.y)/3f;
        transform.position = pos;
        barSprite.sizeDelta = new Vector2(6, 2f);
        borderSprite.sizeDelta = new Vector2(5.88f, 2f); //this width looks better than 160
        bgSprite.sizeDelta = new Vector2(6, 2f);
        //barRect = bar.GetComponent<RectTransform>();
        //originalWidth = barRect.rect.width;
        // // // Vector3 vector = barRect.transform.position;
        // // // vector.x += barRect.rect.xMin;
        Debug.Log("HOIUWEFLOHIUW: "+ barRect.rect.xMin);
        // // // leftPosX = vector.x;//transform.position.x-80*transform.localScale.x;
        //leftPosX = bar.GetComponent<Renderer>().bounds.min.x;
        //hbCanvasTransform.sizeDelta = new Vector2(160, 40);
        //hbCanvas.GetComponent<Image>().transform.localScale = new Vector3(400, 100, 400);
        //bar.localScale = new Vector3(200, 50, 200);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
