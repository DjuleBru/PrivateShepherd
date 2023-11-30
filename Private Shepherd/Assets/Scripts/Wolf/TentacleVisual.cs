using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleVisual : MonoBehaviour
{
    [SerializeField] private GameObject tentacleVisual;
    [SerializeField] private Animator animator;

    [SerializeField] GameObject wolfBitePS;
    [SerializeField] GameObject wolfEatPS;

    private Tentacle tentacle;
    private float attackAnimationHalfDuration = .2f;

    private void Start() {
        tentacleVisual.SetActive(false);
        tentacle = GetComponentInParent<Tentacle>();
        tentacle.OnSheepBite += Tentacle_OnSheepBite;
        tentacle.OnSheepEaten += Tentacle_OnSheepEaten;
        tentacle.OnTentacleEmerge += Tentacle_OnTentacleEmerge;
    }

    private void Tentacle_OnTentacleEmerge(object sender, System.EventArgs e) {
        animator.speed = 1f;
        tentacleVisual.SetActive(true);
    }

    private void Tentacle_OnSheepEaten(object sender, System.EventArgs e) {
        animator.SetTrigger("Attack");
        Invoke("InstantiateEatPS", attackAnimationHalfDuration);
    }

    private void Tentacle_OnSheepBite(object sender, System.EventArgs e) {
        animator.SetTrigger("Attack");
        Invoke("InstantiateBitePS", attackAnimationHalfDuration);
    }


    private void InstantiateBitePS() {
        Instantiate(wolfBitePS, transform.position, Quaternion.identity);
    }

    private void InstantiateEatPS() {
        Instantiate(wolfEatPS, transform.position, Quaternion.identity);
    }
}
