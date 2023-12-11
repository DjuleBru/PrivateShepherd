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
    [SerializeField] GameObject touchSkipButton;

    private bool inputActiveForIntro;
    private bool textShowed;
    private bool androidPort;

    private void Awake() {

        skipButton.fillAmount = 0;

        anyKeyText.gameObject.SetActive(false);
        skipUI.SetActive(false);
    }

    private void Start() {
        androidPort = Player.Instance.GetAndroidPort();
        if (androidPort) {
            anyKeyText.text = "touch to continue";
            skipUI.GetComponentInChildren<TextMeshProUGUI>().text = "hold touch to skip";
            skipButton.gameObject.SetActive(false);
            skipButtonBackGround.gameObject.SetActive(false);
            LevelIntroCutScene.Instance.OnCutSceneExit += LevelCutScene_OnCutSceneExit;
        }
    }

    private void LevelCutScene_OnCutSceneExit(object sender, System.EventArgs e) {
        if (androidPort) {
            touchSkipButton.SetActive(false);
        }
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

        if(!androidPort) {
            HandleSkipButtonUI();
        }

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
