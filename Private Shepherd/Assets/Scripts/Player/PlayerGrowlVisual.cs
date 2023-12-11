using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGrowlVisual : MonoBehaviour
{

    public static PlayerGrowlVisual Instance;

    [SerializeField] private MMF_Player growlMMFPlayer;
    [SerializeField] private WolfSO levelWolfSO;

    private Animator animator;

    [SerializeField] private SpriteRenderer growlWolfAOE;
    [SerializeField] private Image growlTimeUIImage;

    private float barkCircleRadius = 9.25f;

    private float growlTriggerDistance;
    private float growlWolfTriggerDistance;

    private bool growlUnlocked;

    private void Awake() {
        Instance = this;
    }

    private void Start() {

        growlUnlocked = PlayerGrowl.Instance.GetGrowlUnlocked();
        
        PlayerGrowl.Instance.OnPlayerGrowl += Player_OnPlayerGrowl;
        PlayerGrowl.Instance.OnPlayerGrowlReleased += PlayerGrowl_OnPlayerGrowlReleased;
        growlTriggerDistance = PlayerGrowl.Instance.GetGrowlTriggerDistance();

        float growlAOEScaleMultiplier = growlTriggerDistance / barkCircleRadius;
        float growlWolfAOEScaleMultiplier = levelWolfSO.wolfTriggerFleeDistanceMultiplier * growlTriggerDistance / barkCircleRadius;

        growlWolfAOE.transform.localScale = Vector3.one * growlWolfAOEScaleMultiplier;
        growlTimeUIImage.transform.localScale = Vector3.one * growlWolfAOEScaleMultiplier;

        growlWolfAOE.enabled = false;
        growlTimeUIImage.enabled = false;

        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!growlUnlocked) {
            return;
        }

        if (PlayerGrowl.Instance.GetGrowling()) {
            growlTimeUIImage.fillAmount = PlayerGrowl.Instance.GetGrowlTimerNormalized();
        }
        else {
            growlTimeUIImage.fillAmount = 0;
        }
    }

    private void PlayerGrowl_OnPlayerGrowlReleased(object sender, EventArgs e) {
        if (!growlUnlocked) {
            return;
        }

        growlWolfAOE.enabled = false;
        growlTimeUIImage.enabled = false;
        growlMMFPlayer.StopFeedbacks();
        animator.SetBool("isGrowling", false);
    }

    private void Player_OnPlayerGrowl(object sender, EventArgs e) {
        if (!growlUnlocked) {
            return;
        }

        growlWolfAOE.enabled = true;
        growlTimeUIImage.enabled = true;
        growlMMFPlayer.PlayFeedbacks();
        animator.SetBool("isGrowling", true);
    }
}
