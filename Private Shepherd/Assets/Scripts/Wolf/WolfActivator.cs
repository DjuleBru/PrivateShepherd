using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfActivator : MonoBehaviour
{
    [SerializeField] private Wolf wolf;
    [SerializeField] private WhiteWolf whiteWolf;
    [SerializeField] private List<Transform> spawnPoints;

    [SerializeField] private bool isWhiteWolf;

    private float activationTimer;
    [SerializeField] private float activationTime;
    private bool cutSceneInProgress;
    private bool activated;

    public event EventHandler OnWolfActivated;

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
            int i = UnityEngine.Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[i];

            if(isWhiteWolf) {
                whiteWolf.transform.position = spawnPoint.position;
            } else {
                wolf.transform.position = spawnPoint.position;
            }
            activated = true;
            OnWolfActivated?.Invoke(this, EventArgs.Empty);
        }
    }
    private void LevelManager_OnCutSceneExit(object sender, System.EventArgs e) {
        cutSceneInProgress = false;
    }

    private void LevelManager_OnCutSceneEnter(object sender, System.EventArgs e) {
        cutSceneInProgress = true;
    }

}
