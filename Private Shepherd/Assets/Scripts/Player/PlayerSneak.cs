using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSneak : MonoBehaviour
{
    public static PlayerSneak Instance { get; private set; }
    public event EventHandler OnPlayerSneakStarted;
    public event EventHandler OnPlayerSneakReleased;
    [SerializeField] private FleeTarget playerFleeTarget;

    private float initialPlayerMoveSpeed;
    private float initialPlayerFleeTargetSpeedMultiplier;
    private float initialPlayerFleeTargetStopDistance;

    [SerializeField] private float sneakSlowMultiplier;

    private bool sneakUnlocked;
    private bool sneakActive;
    private bool sneaking;

    [SerializeField] bool forceUnlock;

    private void Awake() {
        Instance = this;
    }

    private void Start() {

        sneakUnlocked = ES3.Load("sneakUnlocked", false);

        if(forceUnlock) {
            sneakUnlocked = true;
        }

        GameInput.Instance.OnSneakPerformed += GameInput_OnSneakPerformed;
        GameInput.Instance.OnSneakReleased += GameInput_OnSneakReleased;

        initialPlayerMoveSpeed = PlayerMovement.Instance.GetMoveSpeed();
        initialPlayerFleeTargetSpeedMultiplier = playerFleeTarget.GetFleeTargetSpeedMultiplier();
        initialPlayerFleeTargetStopDistance = playerFleeTarget.GetFleeTargetStopDistance();
    }

    private void GameInput_OnSneakReleased(object sender, EventArgs e) {
        sneaking = false;
        PlayerMovement.Instance.SetMoveSpeed(initialPlayerMoveSpeed);
        playerFleeTarget.SetFleeTargetTriggerDistance(initialPlayerFleeTargetSpeedMultiplier);
        playerFleeTarget.SetFleeTargetStopDistance(initialPlayerFleeTargetStopDistance);

        OnPlayerSneakReleased?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnSneakPerformed(object sender, EventArgs e) {
        if(!sneakUnlocked) {
            return;
        }
        sneaking = true;
        PlayerMovement.Instance.SetMoveSpeed(PlayerMovement.Instance.GetMoveSpeed() * sneakSlowMultiplier);
        playerFleeTarget.SetFleeTargetTriggerDistance(0);
        playerFleeTarget.SetFleeTargetStopDistance(0);

        OnPlayerSneakStarted?.Invoke(this, EventArgs.Empty);
    }

    public void SneakTouchPerformed() {
        if (!sneakUnlocked) {
            return;
        }
        sneaking = true;
        PlayerMovement.Instance.SetMoveSpeed(PlayerMovement.Instance.GetMoveSpeed() * sneakSlowMultiplier);
        playerFleeTarget.SetFleeTargetTriggerDistance(0);
        playerFleeTarget.SetFleeTargetStopDistance(0);

        OnPlayerSneakStarted?.Invoke(this, EventArgs.Empty);
    }

    public void SneakTouchReleased() {
        sneaking = false;
        PlayerMovement.Instance.SetMoveSpeed(initialPlayerMoveSpeed);
        playerFleeTarget.SetFleeTargetTriggerDistance(initialPlayerFleeTargetSpeedMultiplier);
        playerFleeTarget.SetFleeTargetStopDistance(initialPlayerFleeTargetStopDistance);

        OnPlayerSneakReleased?.Invoke(this, EventArgs.Empty);
    }

    public bool GetSneakUnlocked() {
        return sneakUnlocked;
    }

    public void SetSneakUnlocked(bool unlocked) {
        sneakUnlocked = unlocked;
    }

}
