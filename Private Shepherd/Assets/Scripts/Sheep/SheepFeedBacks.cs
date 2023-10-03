using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepFeedBacks : MonoBehaviour
{
    [SerializeField] private SheepMovement sheepMovement;
    [SerializeField] private MMF_Player mmfPlayer;

    private void Start() {
        sheepMovement.OnSheepFlee += SheepMovement_OnSheepFlee;
    }

    private void SheepMovement_OnSheepFlee(object sender, System.EventArgs e) {
        mmfPlayer.PlayFeedbacks();  
    }
}
