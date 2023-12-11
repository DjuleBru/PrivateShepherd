using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepVisual : MonoBehaviour
{
    [SerializeField] SheepMovement sheepMovement;
    [SerializeField] GameObject sheepFleeIndicator;
    [SerializeField] GameObject sheepFleeLeaderIndicator;
    [SerializeField] ParticleSystem sheepInjuredPS;

    private bool blood;

    private void Start() {
        blood = ES3.Load("blood", true);
        SettingsManager.Instance.OnBloodToggled += SettingsManager_OnBloodToggled;

        sheepFleeIndicator.SetActive(false);
        sheepMovement.OnSheepInjured += SheepMovement_OnSheepInjured;
    }
    private void SettingsManager_OnBloodToggled(object sender, System.EventArgs e) {
        blood = blood = ES3.Load("blood", true);
    }

    private void SheepMovement_OnSheepInjured(object sender, System.EventArgs e) {
        if(!blood) {
            return;
        }
        sheepInjuredPS.Play();
    }

    private void Update() {
        if (sheepMovement.GetState() == SheepMovement.State.Flee) {
            sheepFleeIndicator.SetActive(false);
            sheepFleeLeaderIndicator.SetActive(true);
        }
        else if (sheepMovement.GetState() == SheepMovement.State.FleeAggregate) {
            sheepFleeLeaderIndicator.SetActive(false);
            sheepFleeIndicator.SetActive(true);
        }
        else {
            sheepFleeIndicator.SetActive(false);
            sheepFleeLeaderIndicator.SetActive(false);
        }


    }
}
