using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfActivator : MonoBehaviour
{
    [SerializeField] private Wolf wolf;
    [SerializeField] private List<Transform> spawnPoints;

    private float activationTimer;
    [SerializeField] private float activationTime;
    private bool cutSceneInProgress;
    private bool activated;

    private void Start() {
        activationTimer = activationTime;
        LevelManager.Instance.OnCutSceneEnter += LevelManager_OnCutSceneEnter;
        LevelManager.Instance.OnCutSceneExit += LevelManager_OnCutSceneExit;
    }

    private void Update() {
        if(cutSceneInProgress) {
            return;
        }

        activationTimer -= Time.deltaTime;
        if(activationTimer < 0 & !activated) {
            int i = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[i];
            wolf.transform.position = spawnPoint.position;
            activated = true;
        }
    }
    private void LevelManager_OnCutSceneExit(object sender, System.EventArgs e) {
        cutSceneInProgress = false;
    }

    private void LevelManager_OnCutSceneEnter(object sender, System.EventArgs e) {
        cutSceneInProgress = true;
    }

}
