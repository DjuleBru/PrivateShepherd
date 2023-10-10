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

    [SerializeField] private SpriteRenderer growlAOE;
    [SerializeField] private SpriteRenderer growlWolfAOE;
    [SerializeField] private Image growlTimeUIImage;

    private float barkCircleRadius = 9.25f;

    private float growlTriggerDistance;
    private float growlWolfTriggerDistance;

    private void Start() {
        PlayerGrowl.Instance.OnPlayerGrowl += Player_OnPlayerGrowl;
        PlayerGrowl.Instance.OnPlayerGrowlReleased += PlayerGrowl_OnPlayerGrowlReleased;
        growlTriggerDistance = PlayerGrowl.Instance.GetGrowlTriggerDistance();

        float growlAOEScaleMultiplier = growlTriggerDistance / barkCircleRadius;
        float growlWolfAOEScaleMultiplier = levelWolfSO.wolfTriggerFleeDistanceMultiplier * growlTriggerDistance / barkCircleRadius;

        growlAOE.transform.localScale = Vector3.one * growlAOEScaleMultiplier;
        growlWolfAOE.transform.localScale = Vector3.one * growlWolfAOEScaleMultiplier;
        growlTimeUIImage.transform.localScale = Vector3.one * growlWolfAOEScaleMultiplier;

        growlAOE.enabled = false;
        growlWolfAOE.enabled = false;
    }

    private void Update() {
        if (PlayerGrowl.Instance.GetGrowling()) {
            growlTimeUIImage.fillAmount = PlayerGrowl.Instance.GetGrowlTimerNormalized();
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
