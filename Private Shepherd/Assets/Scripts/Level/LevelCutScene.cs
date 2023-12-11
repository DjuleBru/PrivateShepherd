using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelCutScene : MonoBehaviour
{
    [SerializeField] protected SheepObjectPool levelSheepObjectPool;
    [SerializeField] protected GameObject levelPlayUI;
    [SerializeField] protected string cutSceneName;

    [SerializeField] protected float enterTime;
    [SerializeField] protected float exitTime;

    public event EventHandler OnCutSceneEnter;
    public event EventHandler OnCutSceneExit;

    protected bool cutSceneInProgress;

    private float skipTimer;
    private float skipTime = 1f;
    private bool skipping;
    [SerializeField] private bool tutorial;

    protected virtual void Start() {
        GameInput.Instance.OnBarkPerformed += GameInput_OnBarkPerformed;
        GameInput.Instance.OnBarkReleased += GameInput_OnBarkReleased;
    }

    protected virtual void Update() {
        if (skipping & !tutorial) {
            skipTimer += Time.deltaTime;
            if (skipTimer > skipTime) {
                SkipCutScene();
            }
        }
    }
    public virtual void ExitCutScene() {
        if(cutSceneInProgress) {
            OnCutSceneExit?.Invoke(this, EventArgs.Empty);
            PlayerMovement.Instance.SetCanMove(true);
            PlayerBark.Instance.SetBarkActive(true);
            PlayerGrowl.Instance.SetGrowlActive(true);
            PlayerRun.Instance.SetRunActive(true);

            // Activate all UI Elements
            levelSheepObjectPool.ActivateSheepTargetIndicators();
            levelPlayUI.SetActive(true);
        }
        cutSceneInProgress = false;
    }

    public virtual void EnterCutScene() {
        OnCutSceneEnter?.Invoke(this, EventArgs.Empty);
        PlayerMovement.Instance.SetCanMove(false);
        PlayerBark.Instance.SetBarkActive(false);
        PlayerGrowl.Instance.SetGrowlActive(false);
        PlayerRun.Instance.SetRunActive(false);

        // Deactivate all UI Elements
        levelSheepObjectPool.DeactivateSheepTargetIndicators();
        levelPlayUI.SetActive(false);

        cutSceneInProgress = true;
    }

    public void SkipPerformedTouch() {
        skipping = true;
    }

    public void SkipReleasedTouch() {
        skipTimer = 0;
        skipping = false;
    }

    private void GameInput_OnBarkPerformed(object sender, EventArgs e) {
        skipping = true;
    }


    private void GameInput_OnBarkReleased(object sender, EventArgs e) {
        skipTimer = 0;
        skipping = false;
    }

    public virtual void SkipCutScene() {
        ExitCutScene();
        ChangeCinemachineCode.Instance.SetCameraPriority(0);
    }

    public float GetSkipAmountNormalized() {
        return skipTimer / skipTime;
    }

    public bool GetCutSceneInProgress() {
        return cutSceneInProgress;
    }

}
