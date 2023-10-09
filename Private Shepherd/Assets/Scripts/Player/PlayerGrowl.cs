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
    [SerializeField] private float growlCoolDown;

    private float playerFleeTargetSpeedMultiplier;
    private float playerFleeTargetTriggerDistance;
    private float playerFleeTargetStopDistance;

    [SerializeField] private float growlFleeTargetSpeedMultiplier;
    [SerializeField] private float growlFleeTargetTriggerDistance;
    [SerializeField] private float growlFleeTargetStopDistance;

    [SerializeField] private float growlPlayerMoveSpeedMultiplier;
    [SerializeField] private PlayerMovement playerMovement;
    private float initialPlayerMoveSpeed;

    private float growlCoolDownTimer;
    private float growlTimer;
    private bool growling;

    private void Awake() {
        Instance = this;
        growlTimer = growlTime;
    }

    private void Start() {
        playerFleeTargetSpeedMultiplier = playerFleeTarget.GetFleeTargetSpeedMultiplier();
        playerFleeTargetTriggerDistance = playerFleeTarget.GetFleeTargetTriggerDistance();
        playerFleeTargetStopDistance = playerFleeTarget.GetFleeTargetStopDistance();

        GameInput.Instance.OnGrowlPerformed += GameInput_OnGrowlPerformed;
        GameInput.Instance.OnGrowlReleased += GameInput_OnGrowlReleased;

        initialPlayerMoveSpeed = playerMovement.GetMoveSpeed();
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
        if (growlCoolDownTimer < 0) {
            StartGrowl();
        }
    }

    private void StartGrowl() {
        OnPlayerGrowl?.Invoke(this, EventArgs.Empty);
        ModifyFleeTargetParameters(growlFleeTargetTriggerDistance, growlFleeTargetStopDistance);
        playerMovement.SetMoveSpeed(initialPlayerMoveSpeed * growlPlayerMoveSpeedMultiplier);
        growlTimer = growlTime;
        growling = true;
    }

    private void EndGrowl() {
        OnPlayerGrowlReleased?.Invoke(this, EventArgs.Empty);
        ResetFleeTargetParameters();
        playerMovement.SetMoveSpeed(initialPlayerMoveSpeed);

        growlCoolDownTimer = growlCoolDown;
        growling = false;
    }

    private void ModifyFleeTargetParameters(float fleeTargetTriggerDistance, float fleeTargetStopDistance) {
        playerFleeTarget.SetFleeTargetTriggerDistance(fleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(fleeTargetStopDistance);
    }

    private void ResetFleeTargetParameters() {
        playerFleeTarget.SetFleeTargetTriggerDistance(playerFleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(playerFleeTargetStopDistance);
    }

    public float GetGrowlTriggerDistance() {
        return growlFleeTargetTriggerDistance;
    }

}
