using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrowlUI : MonoBehaviour
{
    [SerializeField] Image growlCoolDownImage;

    private Color readyColor;
    private Color loadingColor;
    private void Start() {
        loadingColor = new Color(190/255f, 150 / 255f, 0f / 255f, 255f / 255f);
        readyColor = new Color(96 / 255f, 138 / 255f, 0 / 255f, 255f / 255f);

        growlCoolDownImage.color = readyColor;
    }

    private void Update() {
        float fillAmount = PlayerGrowl.Instance.GetGrowlCoolDownTimerNormalized();
        growlCoolDownImage.fillAmount = fillAmount;

        if (fillAmount < 1) {
            growlCoolDownImage.color = loadingColor;
        } else {
            growlCoolDownImage.color = readyColor;
        }
    }
}
