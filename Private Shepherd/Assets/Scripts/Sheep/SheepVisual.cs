using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepVisual : MonoBehaviour
{
    [SerializeField] SheepMovement sheepMovement;
    [SerializeField] GameObject sheepFleeIndicator;
    [SerializeField] ParticleSystem sheepInjuredPS;

    private void Start() {
        sheepFleeIndicator.SetActive(false);
        sheepMovement.OnSheepInjured += SheepMovement_OnSheepInjured;
    }

    private void SheepMovement_OnSheepInjured(object sender, System.EventArgs e) {
        sheepInjuredPS.Play();
    }

    private void Update() {
        if(sheepMovement.GetState() == SheepMovement.State.Flee | sheepMovement.GetState() == SheepMovement.State.FleeAggregate) {
            sheepFleeIndicator.SetActive(true);
        } else {
            sheepFleeIndicator.SetActive(false);
        }
    }
}
