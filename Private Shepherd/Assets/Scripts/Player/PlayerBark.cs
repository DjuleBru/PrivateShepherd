using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBark : MonoBehaviour
{
    public static PlayerBark Instance { get; private set; }
    public event EventHandler OnPlayerBark;
    [SerializeField] private FleeTarget playerFleeTarget;

    [SerializeField] private float barkCoolDownTimer;
    [SerializeField] private float barkCoolDown = .5f;

    [SerializeField] private float barkFleeTargetSpeedMultiplier;
    [SerializeField] private float barkFleeTargetStopDistance;
    [SerializeField] private float barkFleeTargetTriggerDistance;

    private float playerFleeTargetSpeedMultiplier;
    private float playerFleeTargetTriggerDistance;
    private float playerFleeTargetStopDistance;

    private bool growled;

    private void Awake() {
        Instance = this;
        barkCoolDownTimer = barkCoolDown;
    }

    private void Start() {
        playerFleeTargetSpeedMultiplier = playerFleeTarget.GetFleeTargetSpeedMultiplier();
        playerFleeTargetTriggerDistance = playerFleeTarget.GetFleeTargetTriggerDistance();
        playerFleeTargetStopDistance = playerFleeTarget.GetFleeTargetStopDistance();

        GameInput.Instance.OnBarkPerformed += GameInput_OnBarkPerformed;
    }

    private void Update() {
        barkCoolDownTimer -= Time.deltaTime;
    }

    private void GameInput_OnBarkPerformed(object sender, EventArgs e) {
        if (barkCoolDownTimer < 0) {
            Bark();
        }
    }

    private void Bark() {
        barkCoolDownTimer = barkCoolDown;
        OnPlayerBark?.Invoke(this, EventArgs.Empty);
        StartCoroutine(ModifyPlayerFleeTargetParametersForLimitedTime(barkCoolDown, barkFleeTargetTriggerDistance, barkFleeTargetStopDistance));
        growled = false;
    }

    private void ResetFleeTargetParameters() {
        playerFleeTarget.SetFleeTargetTriggerDistance(playerFleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(playerFleeTargetStopDistance);
    }

    private IEnumerator ModifyPlayerFleeTargetParametersForLimitedTime(float time, float fleeTargetTriggerDistance, float fleeTargetStopDistance) {

        playerFleeTarget.SetFleeTargetTriggerDistance(fleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(fleeTargetStopDistance);

        yield return new WaitForSeconds(time);
        ResetFleeTargetParameters();
    }

    public float GetBarkTriggerDistance() {
        return barkFleeTargetTriggerDistance;
    }
}
