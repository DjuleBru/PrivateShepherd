using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutSceneUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI anyKeyText;
    [SerializeField] private GameObject skipUI;

    [SerializeField] LevelCutScene levelCutScene;
    [SerializeField] Image skipButton;
    [SerializeField] Image skipButtonBackGround;

    private bool inputActiveForIntro;
    private bool textShowed;

    private void Awake() {
        anyKeyText.gameObject.SetActive(false);
        skipUI.SetActive(false);

        skipButton.fillAmount = 0;

    }

    private void Update() {

        inputActiveForIntro = LevelIntroNPC.Instance.GetInputActiveForIntro();
        textShowed = LevelIntroNPC.Instance.GetTextShowed();

        if (!inputActiveForIntro) {
            anyKeyText.gameObject.SetActive(false);
            skipUI.SetActive(false);
            return;
        } else {
            anyKeyText.gameObject.SetActive(true);
            skipUI.SetActive(true);
        }

        HandleSkipButtonUI();

        if (textShowed) {
            anyKeyText.gameObject.SetActive(true);
        }
        else {
            anyKeyText.gameObject.SetActive(false);
        }

    }

    private void HandleSkipButtonUI() {
        skipButton.sprite = InputSpriteManager.Instance.GetBarkSprite();
        skipButtonBackGround.sprite = InputSpriteManager.Instance.GetBarkSprite();

        skipButton.transform.localScale = InputSpriteManager.Instance.GetBarkSpriteScale();
        skipButtonBackGround.transform.localScale = InputSpriteManager.Instance.GetBarkSpriteScale();

        skipButton.fillAmount = levelCutScene.GetSkipAmountNormalized();
    }
}
