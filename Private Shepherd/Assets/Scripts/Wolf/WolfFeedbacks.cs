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

    private float attackAnimationHalfDuration = .2f;

    private void Start() {
        wolfAI.OnSheepBite += WolfAI_OnSheepBite;
        wolfAI.OnSheepEaten += WolfAI_OnSheepEaten;
    }

    private void WolfAI_OnSheepEaten(object sender, System.EventArgs e) {
        wolfAnimator.SetTrigger("Attack");
        Invoke("InstantiateEatPS", attackAnimationHalfDuration);
    }

    private void WolfAI_OnSheepBite(object sender, System.EventArgs e) {
        wolfAnimator.SetTrigger("Attack");
        Invoke("InstantiateBitePS", attackAnimationHalfDuration);
    }

    private void InstantiateBitePS() {
        Instantiate(wolfBitePS, transform.position, Quaternion.identity);
    }

    private void InstantiateEatPS() {
        Instantiate(wolfEatPS, transform.position, Quaternion.identity);
    }
}
