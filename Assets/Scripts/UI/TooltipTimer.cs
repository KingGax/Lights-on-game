using UnityEngine;

[RequireComponent(typeof(Tooltip))]
public class TooltipTimer : MonoBehaviour {

    public float wait = 0;
    public float duration = 0;

    public void Awake() {
        Invoke("Show", wait);
    }

    private void Show() {
        Tooltip t = GetComponent<Tooltip>();
        t.Open();
        Invoke("Hide", duration);
    }

    private void Hide() {
        Tooltip t = GetComponent<Tooltip>();
        t.Dismiss();
    }
}