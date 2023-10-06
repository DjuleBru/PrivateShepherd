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
    [SerializeField] private float barkCoolDown = 1;
    [SerializeField] private float barkFleeTargetSpeedMultiplier = 2f;
    [SerializeField] private float barkFleeTargetStopDistance = 4f;
    [SerializeField] private float barkFleeTargetTriggerDistance = 4f;

    float playerFleeTargetSpeedMultiplier;
    float playerFleeTargetTriggerDistance;

    private void Awake() {
        Instance = this;
        barkCoolDownTimer = barkCoolDown;
    }

    private void Start() {
        playerFleeTargetSpeedMultiplier = playerFleeTarget.GetFleeTargetSpeedMultiplier();
        playerFleeTargetTriggerDistance = playerFleeTarget.GetFleeTargetTriggerDistance();
        GameInput.Instance.OnBarkPerformed += Instance_OnBarkPerformed;
    }


    private void Update() {
        barkCoolDownTimer -= Time.deltaTime;
    }

    private void Instance_OnBarkPerformed(object sender, EventArgs e) {
        if (barkCoolDownTimer < 0) {
            Bark();
        }
    }
    private void Bark() {
        OnPlayerBark?.Invoke(this, EventArgs.Empty);
        barkCoolDownTimer = barkCoolDown;
        StartCoroutine(ModifyPlayerFleeTargetParameters());
    }


    private IEnumerator ModifyPlayerFleeTargetParameters() {

        playerFleeTarget.SetFleeTargetTriggerDistance(barkFleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(barkFleeTargetStopDistance);

        yield return new WaitForSeconds(barkCoolDown);

        playerFleeTarget.SetFleeTargetTriggerDistance(playerFleeTargetTriggerDistance);

    }
}
