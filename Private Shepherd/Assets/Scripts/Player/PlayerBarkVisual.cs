using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerBarkVisual : MonoBehaviour
{

    [SerializeField] private MMF_Player mmfPlayer;

    private void Start() {
        PlayerBark.Instance.OnPlayerBark += PlayerBark_OnPlayerBark;
    }

    private void PlayerBark_OnPlayerBark(object sender, EventArgs e) {
        mmfPlayer.PlayFeedbacks();
       // barkPS.Play();
    }
}
