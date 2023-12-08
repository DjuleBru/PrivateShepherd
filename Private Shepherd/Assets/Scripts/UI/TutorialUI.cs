using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI anyKeyText;
    private bool inputActiveForIntro;
    private bool textShowed;

    private void Awake() {
        anyKeyText.gameObject.SetActive(false);

    }

    private void Update() {

        inputActiveForIntro = LevelIntroNPC.Instance.GetInputActiveForIntro();
        textShowed = LevelIntroNPC.Instance.GetTextShowed();

        if (!inputActiveForIntro) {
            anyKeyText.gameObject.SetActive(false);
            return;
        }
        else {
            anyKeyText.gameObject.SetActive(true);
        }

        if (textShowed) {
            anyKeyText.gameObject.SetActive(true);
        }
        else {
            anyKeyText.gameObject.SetActive(false);
        }

    }

}
