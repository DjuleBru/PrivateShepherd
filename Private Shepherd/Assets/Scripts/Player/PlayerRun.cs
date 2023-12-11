using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    public static PlayerRun Instance { get; private set; }

    [SerializeField] float runSpeedMultiplier;
    [SerializeField] float runMaxTime;
    [SerializeField] float runTiredTimeFraction;
    [SerializeField] float runExtremelyTiredTimeFraction;

    [SerializeField] float tiredRunSpeedMultiplier;

    [SerializeField] AudioSource tiredAudioSource;

    public event EventHandler OnPlayerRun;
    public event EventHandler OnPlayerRunStop;
    public event EventHandler onPlayerTired;
    public event EventHandler onPlayerRecovered;

    float runTiredTime;
    float runExtremelyTiredTime;
    private float recoverFactor = .5f;
    private float playerMoveSpeed;

    private float runTimer;

    private bool running;
    private bool tired;
    private bool extremelyTired;

    private bool runUnlocked;
    private bool runActive;

    [SerializeField] bool forceUnlock;



    private void Awake() {
        Instance = this;
    }

    private void Start() {


        runUnlocked = ES3.Load("runUnlocked", false);

        if(forceUnlock) {
            runUnlocked = true;
        }

        GameInput.Instance.OnRunPerformed += GameInput_OnRunPerformed;
        GameInput.Instance.OnRunReleased += GameInput_OnRunReleased;

        playerMoveSpeed = PlayerMovement.Instance.GetMoveSpeed();

        runTiredTime = runMaxTime * runTiredTimeFraction;
        runExtremelyTiredTime = runMaxTime * runExtremelyTiredTimeFraction;
        runTimer = 0;
    }
    private void Update() {

        if (!runUnlocked) {
            return;
        }

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

        if (runTimer > runTiredTime & !tired) {
            tired = true;
            Debug.Log(tired);
            onPlayerTired?.Invoke(this, EventArgs.Empty);
        } else if (runTimer < runTiredTime & tired) {
            tired = false;
            if (!extremelyTired) { 
                onPlayerRecovered?.Invoke(this, EventArgs.Empty);
            }
        }

        if (runTimer >= runExtremelyTiredTime) {
            extremelyTired = true;
            if (!running) {
                PlayerMovement.Instance.SetMoveSpeed(playerMoveSpeed * tiredRunSpeedMultiplier);
            }
        } else if (runTimer <= 0 & extremelyTired) {
            extremelyTired = false;
            onPlayerRecovered?.Invoke(this, EventArgs.Empty);
            PlayerMovement.Instance.SetMoveSpeed(playerMoveSpeed);
        }

    }

    private void GameInput_OnRunReleased(object sender, System.EventArgs e) {
        if (running) {
            StopRunning();
            OnPlayerRunStop?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnRunPerformed(object sender, System.EventArgs e) {

        if (runActive & runUnlocked) {
            if (!tired & !extremelyTired)
                Run();
        }
    }

    public void RunTouchPerformed() {
        if (runActive & runUnlocked) {
            if (!tired & !extremelyTired)
                Run();
        }
    }

    public void RunTouchReleased() {
        if (running) {
            StopRunning();
            OnPlayerRunStop?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Run() {
        if(!running) {
            OnPlayerRun?.Invoke(this, EventArgs.Empty);
        }
        running = true;

        PlayerMovement.Instance.SetMoveSpeed(playerMoveSpeed * runSpeedMultiplier);
    }

    private void StopRunning() {
        running = false;
        if (!extremelyTired) {
            PlayerMovement.Instance.SetMoveSpeed(playerMoveSpeed);
        }
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

    public bool GetRunUnlocked() {
        return runUnlocked;
    }

    public void SetRunUnlocked(bool unlocked) {
        runUnlocked = unlocked;
    }

    public void SetRunActive(bool active) {
        runActive = active;
    }

    public AudioSource GetAudioSource() {
        return tiredAudioSource;
    }

}
