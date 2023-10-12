using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfFeedbacks : MonoBehaviour
{
    [SerializeField] WolfAI wolfAI;
    [SerializeField] GameObject wolfBitePS;
    [SerializeField] GameObject wolfEatPS;
    [SerializeField] Animator wolfAnimator;

    [SerializeField] MMF_Player wolfBiteFeedBacks;
    [SerializeField] MMF_Player wolfEatFeedBacks;
    [SerializeField] MMF_Player wolfFleeFeedBacks;

    private float attackAnimationHalfDuration = .2f;

    private void Start() {
        wolfAI.OnSheepBite += WolfAI_OnSheepBite;
        wolfAI.OnSheepEaten += WolfAI_OnSheepEaten;
        wolfAI.OnWolfFlee += WolfAI_OnWolfFlee;
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
        Instantiate(wolfBitePS, transform.position, Quaternion.identity);
    }

    private void InstantiateEatPS() {
        Instantiate(wolfEatPS, transform.position, Quaternion.identity);
    }
}
