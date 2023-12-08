using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelCompleteUI : MonoBehaviour
{

    private float levelCompleteTimer = 1f;

    void Update() {
        levelCompleteTimer -= Time.deltaTime;
        if (levelCompleteTimer < 0) {
            if (Input.anyKeyDown) {
                ES3.Save("Tutorial_completed", true);
                SceneLoader.Instance.LoadWorldMap();
            }
        }
    }
}
