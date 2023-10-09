using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrowlVisual : MonoBehaviour
{

    [SerializeField] private MMF_Player growlMMFPlayer;
    [SerializeField] private WolfSO levelWolfSO;


    [SerializeField] private SpriteRenderer growlAOE;
    [SerializeField] private SpriteRenderer growlWolfAOE;

    private float growlTriggerDistance;
    private float growlWolfTriggerDistance;

    private void Start() {
        PlayerGrowl.Instance.OnPlayerGrowl += Player_OnPlayerGrowl;
        PlayerGrowl.Instance.OnPlayerGrowlReleased += PlayerGrowl_OnPlayerGrowlReleased;
        growlTriggerDistance = PlayerGrowl.Instance.GetGrowlTriggerDistance();

        float growlAOEScaleMultiplier = growlTriggerDistance / 3.5f;
        float growlWolfAOEScaleMultiplier = levelWolfSO.wolfTriggerFleeDistanceMultiplier * growlTriggerDistance / 3.5f;

        growlAOE.transform.localScale = Vector3.one * growlAOEScaleMultiplier;
        growlWolfAOE.transform.localScale = Vector3.one * growlWolfAOEScaleMultiplier;

        growlAOE.enabled = false;
        growlWolfAOE.enabled = false;
    }

    private void PlayerGrowl_OnPlayerGrowlReleased(object sender, EventArgs e) {
        growlAOE.enabled = false;
        growlWolfAOE.enabled = false;
        growlMMFPlayer.StopFeedbacks();
    }

    private void Player_OnPlayerGrowl(object sender, EventArgs e) {
        growlAOE.enabled = true;
        growlWolfAOE.enabled = true;
        growlMMFPlayer.PlayFeedbacks();
    }
}
