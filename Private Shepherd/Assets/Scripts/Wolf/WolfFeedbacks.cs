using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfFeedbacks : MonoBehaviour
{
    [SerializeField] WolfAI wolfAI;
    [SerializeField] GameObject wolfBitePS;
    [SerializeField] GameObject wolfEatPS;
    [SerializeField] ParticleSystem wolfPoisonedPS;
    [SerializeField] Animator wolfAnimator;

    [SerializeField] MMF_Player wolfBiteFeedBacks;
    [SerializeField] MMF_Player wolfEatFeedBacks;
    [SerializeField] MMF_Player wolfFleeFeedBacks;

    private float attackAnimationHalfDuration = .2f;

    private bool blood;

    private void Start() {
        blood = ES3.Load("blood", true);
        SettingsManager.Instance.OnBloodToggled += SettingsManager_OnBloodToggled;

        wolfAI.OnSheepBite += WolfAI_OnSheepBite;
        wolfAI.OnSheepEaten += WolfAI_OnSheepEaten;
        wolfAI.OnWolfFlee += WolfAI_OnWolfFlee;
        wolfAI.OnWolfDied += WolfAI_OnWolfDied;
        wolfAI.OnWolfPoisoned += WolfAI_OnWolfPoisoned;
    }

    private void SettingsManager_OnBloodToggled(object sender, System.EventArgs e) {
        blood = blood = ES3.Load("blood", true);
    }

    private void WolfAI_OnWolfPoisoned(object sender, System.EventArgs e) {
        wolfPoisonedPS.Play();
    }

    private void WolfAI_OnWolfDied(object sender, WolfAI.OnWolfDiedEventArgs e) {
        wolfAnimator.SetTrigger("Die");
    }

    private void WolfAI_OnWolfFlee(object sender, System.EventArgs e) {
        wolfFleeFeedBacks.PlayFeedbacks();
    }

    private void WolfAI_OnSheepEaten(object sender, System.EventArgs e) {
        wolfAnimator.SetTrigger("Attack");
        wolfEatFeedBacks.PlayFeedbacks();
        Invoke("InstantiateEatPS", attackAnimationHalfDuration);
    }

    private void WolfAI_OnSheepBite(object sender, System.EventArgs e) {
        wolfAnimator.SetTrigger("Attack");
        wolfBiteFeedBacks.PlayFeedbacks();
        Invoke("InstantiateBitePS", attackAnimationHalfDuration);
    }

    private void InstantiateBitePS() {
        if(!blood) {
            return;
        }
        Instantiate(wolfBitePS, transform.position, Quaternion.identity);
    }

    private void InstantiateEatPS() {
        if (!blood) {
            return;
        }
        Instantiate(wolfEatPS, transform.position, Quaternion.identity);
    }
}
