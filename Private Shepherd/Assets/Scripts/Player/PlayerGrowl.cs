using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGrowl : MonoBehaviour
{
    public static PlayerGrowl Instance { get; private set; }

    public event EventHandler OnPlayerGrowl;
    public event EventHandler OnPlayerGrowlReleased;
    [SerializeField] private FleeTarget playerFleeTarget;

    [SerializeField] private float growlTime;
    [SerializeField] private float growlCoolDownTime;

    private float playerFleeTargetSpeedMultiplier;
    private float playerFleeTargetTriggerDistance;
    private float playerFleeTargetStopDistance;

    [SerializeField] private float growlFleeTargetSpeedMultiplier;
    [SerializeField] private float growlFleeTargetTriggerDistance;
    [SerializeField] private float growlFleeTargetStopDistance;

    [SerializeField] private float growlPlayerMoveSpeedMultiplier;
    private float initialPlayerMoveSpeed;

    private float growlCoolDownTimer;
    private float growlTimer;
    private bool growling;

    private bool growlUnlocked = true;
    private bool growlActive;

    private void Awake() {
        Instance = this;
        growlTimer = growlTime;
        growlCoolDownTimer = 0;
    }

    private void Start() {
        playerFleeTargetSpeedMultiplier = playerFleeTarget.GetFleeTargetSpeedMultiplier();
        playerFleeTargetTriggerDistance = playerFleeTarget.GetFleeTargetTriggerDistance();
        playerFleeTargetStopDistance = playerFleeTarget.GetFleeTargetStopDistance();

        GameInput.Instance.OnGrowlPerformed += GameInput_OnGrowlPerformed;
        GameInput.Instance.OnGrowlReleased += GameInput_OnGrowlReleased;

        initialPlayerMoveSpeed = PlayerMovement.Instance.GetMoveSpeed();
    }

    private void Update() {
        growlTimer -= Time.deltaTime;
        growlCoolDownTimer -= Time.deltaTime;

        if (growlTimer < 0 & growling) {
            EndGrowl();
        }
    }

    private void GameInput_OnGrowlReleased(object sender, EventArgs e) {
        if (growling) {
            EndGrowl();
        }
    }

    private void GameInput_OnGrowlPerformed(object sender, EventArgs e) {
        if(growlActive & growlUnlocked) {
            if (growlCoolDownTimer < 0) {
                StartGrowl();
            }
        }
    }

    private void StartGrowl() {
        OnPlayerGrowl?.Invoke(this, EventArgs.Empty);
        ModifyFleeTargetParameters(growlFleeTargetTriggerDistance, growlFleeTargetStopDistance, growlFleeTargetSpeedMultiplier);
        PlayerMovement.Instance.SetMoveSpeed(initialPlayerMoveSpeed * growlPlayerMoveSpeedMultiplier);
        growlTimer = growlTime;
        growling = true;
    }

    private void EndGrowl() {
        OnPlayerGrowlReleased?.Invoke(this, EventArgs.Empty);
        ResetFleeTargetParameters();
        PlayerMovement.Instance.SetMoveSpeed(initialPlayerMoveSpeed);

        growlCoolDownTimer = growlCoolDownTime;
        growling = false;
    }

    private void ModifyFleeTargetParameters(float fleeTargetTriggerDistance, float fleeTargetStopDistance, float fleeTargetSpeedMultiplier) {
        playerFleeTarget.SetFleeTargetTriggerDistance(fleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(fleeTargetStopDistance);
        playerFleeTarget.SetFleeTargetSpeedMultiplier(fleeTargetSpeedMultiplier);
    }

    private void ResetFleeTargetParameters() {
        playerFleeTarget.SetFleeTargetTriggerDistance(playerFleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(playerFleeTargetStopDistance);
        playerFleeTarget.SetFleeTargetSpeedMultiplier(playerFleeTargetSpeedMultiplier);
    }

    public float GetGrowlTriggerDistance() {
        return growlFleeTargetTriggerDistance;
    }

    public float GetGrowlCoolDownTimerNormalized() {
        return (1 - (growlCoolDownTimer/growlCoolDownTime));
    }

    public float GetGrowlTimerNormalized() {
        return (growlTimer / growlTime);
    }

    public bool GetGrowling() {
        return growling;
    }

    public void SetGrowlUnlocked(bool unlocked) {
        growlUnlocked = unlocked;
    }

    public void SetGrowlActive(bool active) {
        growlActive = active;
    }

}
