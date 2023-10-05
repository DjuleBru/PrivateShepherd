using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LevelTimerUI : MonoBehaviour
{
    private float levelTimeLimit;
    private float levelTimer;

    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private Animator clockNeedleAnimator;
    [SerializeField] private Animator clockShadowAnimator;


    private void Awake() {
        clockNeedleAnimator.enabled = false;
        clockShadowAnimator.enabled = false;
    }

    private void Start() {
        levelTimeLimit = LevelManager.Instance.GetLevelTimeLimit();
        levelTimer = LevelManager.Instance.GetLevelTimer();
        ActivateClock();
    }

    private void Update() {
        levelTimer -= Time.deltaTime;
        UpdateClockText();
    }

    private void ActivateClock() {
        float levelFrameNumber = levelTimeLimit * 60;
        // Animation is composed of 15 frames
        float animationSpeed = 15 / levelFrameNumber;

        clockNeedleAnimator.speed = animationSpeed;
        clockShadowAnimator.speed = animationSpeed;
        clockNeedleAnimator.enabled = true;
        clockShadowAnimator.enabled = true;
    }

    private void UpdateClockText() {
        if (levelTimer > 10) {
            clockText.text = ((int)Math.Floor(levelTimer)).ToString();
        } else {
            string levelTimerText = (Math.Truncate(100 * levelTimer) / 100).ToString();
            clockText.text = levelTimerText;
        }
    }

}
