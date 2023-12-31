using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarkUI : MonoBehaviour
{
    [SerializeField] Image barkCoolDownImage;
    [SerializeField] GameObject abilitiesPanelUI;

    private Color readyColor;
    private Color loadingColor;

    private bool barkUnlocked;

    private void Start() {

        barkUnlocked = PlayerBark.Instance.GetBarkUnlocked();
        if (!barkUnlocked) {
            gameObject.SetActive(false);
        } else {
            abilitiesPanelUI.SetActive(true);
        }

        loadingColor = new Color(190 / 255f, 150 / 255f, 0f / 255f, 255f / 255f);
        readyColor = new Color(96 / 255f, 138 / 255f, 0 / 255f, 255f / 255f);

        barkCoolDownImage.color = readyColor;
    }

    private void Update() {
        float fillAmount = PlayerBark.Instance.GetBarkCoolDownTimerNormalized();
        barkCoolDownImage.fillAmount = fillAmount;

        if (fillAmount < 1) {
            barkCoolDownImage.color = loadingColor;
        }
        else {
            barkCoolDownImage.color = readyColor;
        }
    }
}
