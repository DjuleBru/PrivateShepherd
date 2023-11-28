using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipIconUI : MonoBehaviour
{
    [SerializeField] LevelCutScene levelCutScene;
    [SerializeField] Image skipButton;

    private void Start() {
        skipButton.fillAmount = 0;
    }

    private void Update() {
        skipButton.fillAmount = levelCutScene.GetSkipAmountNormalized();
    }
}
