using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PlayerBarkVisual : MonoBehaviour
{

    [SerializeField] private MMF_Player barkMMFPlayer;
    [SerializeField] private SpriteRenderer barkAOE;
    [SerializeField] private SpriteRenderer barkWolfAOE;

    [SerializeField] private WolfSO levelWolfSO;

    [SerializeField] private float initialBarkVisualAlpha;

    private float barkCircleRadius = 9.25f;

    private float barkTriggerDistance;
    private float barkWolfTriggerDistance;

    private bool barkUnlocked;

    private void Start() {
        barkUnlocked = PlayerBark.Instance.GetBarkUnlocked();

        PlayerBark.Instance.OnPlayerBark += PlayerBark_OnPlayerBark;
        barkTriggerDistance = PlayerBark.Instance.GetBarkTriggerDistance();

        float barkAOEScaleMultiplier = barkTriggerDistance / barkCircleRadius;
        float barkWolfAOEScaleMultiplier = levelWolfSO.wolfTriggerFleeDistanceMultiplier * barkTriggerDistance / barkCircleRadius;

        barkAOE.transform.localScale = Vector3.one * barkAOEScaleMultiplier;
        barkWolfAOE.transform.localScale = Vector3.one * barkWolfAOEScaleMultiplier;

        barkAOE.enabled = false;
        barkWolfAOE.enabled = false;
    }

    private void PlayerBark_OnPlayerBark(object sender, EventArgs e) {
        if (!barkUnlocked) {
            return;
        }

        barkAOE.enabled = true;
        barkWolfAOE.enabled = true;
        barkMMFPlayer.PlayFeedbacks();
        StartCoroutine(FadeBarkAOEVisuals());
    }

    private IEnumerator FadeBarkAOEVisuals() {
        Color barkColortmp = barkAOE.color;
        Color barkWolfColortmp = barkWolfAOE.color;

        float alpha = initialBarkVisualAlpha;
        
        while (alpha >= 0) {
            barkColortmp.a = alpha;
            barkWolfColortmp.a = alpha;

            barkAOE.color = barkColortmp;
            barkWolfAOE.color = barkWolfColortmp;

            alpha -= .1f;

            yield return new WaitForSeconds(.05f);
        }
        barkAOE.enabled = false;
        barkWolfAOE.enabled = false;
    }
}
