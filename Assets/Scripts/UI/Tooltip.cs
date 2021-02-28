using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    public Transform target;

    public void Update() {

        transform.position = Camera.main.WorldToScreenPoint(target.position);
    }
}