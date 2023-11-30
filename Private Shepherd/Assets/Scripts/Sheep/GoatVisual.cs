using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatVisual : MonoBehaviour
{
    [SerializeField] GameObject goatRamIndicator;
    [SerializeField] ParticleSystem goatRam;

    private SheepMovement sheepMovement;
    private Animator animator;

    private void Awake() {
        sheepMovement = GetComponentInParent<SheepMovement>();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        goatRamIndicator.SetActive(false);
        sheepMovement.OnGoatRam += SheepMovement_OnGoatRam;
        sheepMovement.OnGoatRamWarning += SheepMovement_OnGoatRamWarning;
        sheepMovement.OnGoatRamEnd += SheepMovement_OnGoatRamEnd;
    }

    private void SheepMovement_OnGoatRamEnd(object sender, System.EventArgs e) {
        goatRam.Stop();
        animator.speed = 1f;
    }

    private void SheepMovement_OnGoatRamWarning(object sender, System.EventArgs e) {
        goatRamIndicator.SetActive(true);
    }
    private void SheepMovement_OnGoatRam(object sender, System.EventArgs e) {
        goatRamIndicator.SetActive(false);
        goatRam.Play();
        animator.speed = 2f;
    }
}
