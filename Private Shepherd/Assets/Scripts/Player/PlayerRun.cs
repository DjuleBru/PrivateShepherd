using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    public static PlayerRun Instance { get; private set; }

    [SerializeField] float runSpeedMultiplier;
    [SerializeField] float runMaxTime;
    [SerializeField] float runTiredTime;
    [SerializeField] float runExtremelyTiredTime;
    [SerializeField] int runCoolDownTime;

    [SerializeField] PlayerMovement playerMovement;

    public event EventHandler onPlayerRun;
    public event EventHandler onPlayerStopRun;
    public event EventHandler onPlayerTired;
    public event EventHandler onPlayerRecovered;

    private float recoverFactor = .5f;
    private float playerMoveSpeed;

    private float runTimer;

    private bool running;
    private bool tired;
    private bool extremelyTired;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        GameInput.Instance.OnRunPerformed += GameInput_OnRunPerformed;
        GameInput.Instance.OnRunReleased += GameInput_OnRunReleased;

        playerMoveSpeed = playerMovement.GetMoveSpeed();

        runTimer = 0;
    }
    private void Update() {

        if (running) {
            runTimer += Time.deltaTime;
        } else {
            if (runTimer >= 0) {
                runTimer -= Time.deltaTime * recoverFactor;
            }
        }

        if (running & runTimer >= runMaxTime) {
            StopRunning();
        }

        if (runTimer > runTiredTime) {
            tired = true;
        } else {
            tired = false;
        }

        if (runTimer >= runExtremelyTiredTime) {
            extremelyTired = true;
        } else if (runTimer <= 0) {
            extremelyTired = false;
        }

    }

    private void GameInput_OnRunReleased(object sender, System.EventArgs e) {
        if (running) {
            StopRunning();
        }
    }

    private void GameInput_OnRunPerformed(object sender, System.EventArgs e) {
        if (!tired & !extremelyTired)
            Run();
    }

    private void Run() {
        running = true;

        playerMovement.SetMoveSpeed(playerMoveSpeed * runSpeedMultiplier);
        onPlayerRun?.Invoke(this, EventArgs.Empty);
    }

    private void StopRunning() {
        running = false;

        playerMovement.SetMoveSpeed(playerMoveSpeed);
        onPlayerStopRun?.Invoke(this, EventArgs.Empty);
    }

    public bool GetTired() {
        return tired;
    }

    public bool GetExtremelyTired() {
        return extremelyTired;
    }

    public float GetRunProgression() {
        return (runTimer / runMaxTime);
    }

}
