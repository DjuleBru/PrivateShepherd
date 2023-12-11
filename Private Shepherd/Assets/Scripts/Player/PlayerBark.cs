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

    [SerializeField] private float barkCoolDownTime;
    [SerializeField] private float barkEffetDuration;
    private float barkCoolDownTimer;

    [SerializeField] private float barkFleeTargetSpeedMultiplier;
    [SerializeField] private float barkFleeTargetStopDistance;
    [SerializeField] private float barkFleeTargetTriggerDistance;

    private float playerFleeTargetSpeedMultiplier;
    private float playerFleeTargetTriggerDistance;
    private float playerFleeTargetStopDistance;

    private bool barkUnlocked;
    private bool barkActive;

    [SerializeField] bool forceUnlock;

    private void Awake() {
        Instance = this;
        barkCoolDownTimer = 0;
    }

    private void Start() {
        barkUnlocked = ES3.Load("barkUnlocked", false);

        if(forceUnlock) {
            barkUnlocked = true;
        }

        playerFleeTargetSpeedMultiplier = playerFleeTarget.GetFleeTargetSpeedMultiplier();
        playerFleeTargetTriggerDistance = playerFleeTarget.GetFleeTargetTriggerDistance();
        playerFleeTargetStopDistance = playerFleeTarget.GetFleeTargetStopDistance();

        GameInput.Instance.OnBarkPerformed += GameInput_OnBarkPerformed;
    }

    private void Update() {
        barkCoolDownTimer -= Time.deltaTime;
    }

    public void BarkTouch() {
        if (barkUnlocked & barkActive) {
            if (barkCoolDownTimer < 0) {
                Bark();
            }
        }
    }

    private void GameInput_OnBarkPerformed(object sender, EventArgs e) {
        if(barkUnlocked & barkActive) {
            if (barkCoolDownTimer < 0) {
                Bark();
            }
        }
    }

    private void Bark() {
        barkCoolDownTimer = barkCoolDownTime;
        OnPlayerBark?.Invoke(this, EventArgs.Empty);
        StartCoroutine(ModifyPlayerFleeTargetParametersForLimitedTime(barkEffetDuration, barkFleeTargetTriggerDistance, barkFleeTargetStopDistance, barkFleeTargetSpeedMultiplier));
    }

    private void ResetFleeTargetStopDistance() {
        playerFleeTarget.SetFleeTargetStopDistance(playerFleeTargetStopDistance);
    }

    private void ResetFleeTargetTriggerDistance() {
        playerFleeTarget.SetFleeTargetTriggerDistance(playerFleeTargetTriggerDistance);
    }

    private void ResetFleeTargetSpeedMultiplier() {
        playerFleeTarget.SetFleeTargetSpeedMultiplier(playerFleeTargetSpeedMultiplier);
    }

    private IEnumerator ModifyPlayerFleeTargetParametersForLimitedTime(float time, float fleeTargetTriggerDistance, float fleeTargetStopDistance, float fleeTargetSpeedMultiplier) {

        playerFleeTarget.SetFleeTargetTriggerDistance(fleeTargetTriggerDistance);
        playerFleeTarget.SetFleeTargetStopDistance(fleeTargetStopDistance);
        playerFleeTarget.SetFleeTargetSpeedMultiplier(fleeTargetSpeedMultiplier);

        yield return new WaitForSeconds(.1f);
        ResetFleeTargetTriggerDistance();

        yield return new WaitForSeconds(time);
        ResetFleeTargetStopDistance();
        ResetFleeTargetSpeedMultiplier();
    }

    public float GetBarkTriggerDistance() {
        return barkFleeTargetTriggerDistance;
    }

    public float GetBarkCoolDownTimerNormalized() {
        return (1- (barkCoolDownTimer/ barkCoolDownTime));
    }

    public void SetBarkUnlocked(bool unlocked) {
        Debug.Log("Bark set to unlocked");
        barkUnlocked = unlocked;
    }

    public void SetBarkActive(bool active) {
        barkActive = active;
    }

    public bool GetBarkUnlocked() {
        return barkUnlocked;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.GetComponent<QuestGiver>() != null) {
            barkActive = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<QuestGiver>() != null) {
            barkActive = true;
        }
    }
}
