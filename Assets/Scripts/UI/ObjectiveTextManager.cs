using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ObjectiveType {
    FollowArrow,
    KillEnemies,
}
public class ObjectiveTextManager : MonoBehaviour {
    ObjectiveType objective;
    bool textChanged = true;
    TextMeshProUGUI text;
    int enemiesLeft;
    SetSpawnRoom objectiveRoom;
    // Start is called before the first frame update
    void Start() {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        if (textChanged) {
            UpdateText();
        }
    }

    public void RefreshEnemyObjective(int enemyNum) {
        objective = ObjectiveType.KillEnemies;
        enemiesLeft = enemyNum;
        textChanged = true;
    }

    void UpdateText() {
        switch (objective) {
            case ObjectiveType.FollowArrow:
                text.SetText("Objective: Follow navigation arrow");
                break;
            case ObjectiveType.KillEnemies:
                text.SetText("Objective: Kill " + enemiesLeft + " enemies");
                break;
            default:
                break;
        }
    }
}
