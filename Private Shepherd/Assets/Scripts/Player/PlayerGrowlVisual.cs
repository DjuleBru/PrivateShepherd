using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGrowlVisual : MonoBehaviour
{

    [SerializeField] private MMF_Player growlMMFPlayer;
    [SerializeField] private WolfSO levelWolfSO;
    [SerializeField] private PlayerGrowl playerGrowl;

    [SerializeField] private SpriteRenderer growlAOE;
    [SerializeField] private SpriteRenderer growlWolfAOE;
    [SerializeField] private Image growlTimeUIImage;

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

    private void Update() {
        if (playerGrowl.GetGrowling()) {
            growlTimeUIImage.fillAmount = playerGrowl.GetGrowlTimerNormalized();
        }
        else {
            growlTimeUIImage.fillAmount = 0;
        }
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
