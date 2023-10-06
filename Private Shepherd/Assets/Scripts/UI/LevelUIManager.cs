using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] GameObject levelFailedUI;
    [SerializeField] GameObject levelPlayUI;
    [SerializeField] GameObject levelSuccededUI;

    [SerializeField] LevelManager levelManager;

    private void Awake() {
        levelFailedUI.SetActive(false);
        levelPlayUI.SetActive(true);
        levelSuccededUI.SetActive(false);
    }

    private void Start() {
        levelManager.OnLevelSucceeded += LevelManager_OnLevelSucceeded;
        levelManager.OnLevelFailed += LevelManager_OnLevelFailed;
    }

    private void LevelManager_OnLevelFailed(object sender, System.EventArgs e) {
        levelPlayUI.SetActive(false);
        levelFailedUI.SetActive(true);
    }

    private void LevelManager_OnLevelSucceeded(object sender, LevelManager.OnLevelSucceededEventArgs e) {
        levelPlayUI.SetActive(false);
        levelSuccededUI.SetActive(true);
    }
}
