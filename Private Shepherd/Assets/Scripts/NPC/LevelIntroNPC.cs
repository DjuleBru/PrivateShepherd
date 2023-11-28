using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelIntroNPC : MonoBehaviour
{

    public List<string> phrases = new List<string>();
    [SerializeField] private TextMeshProUGUI phraseText;
    [SerializeField] private TextMeshProUGUI anyKeyText;

    [SerializeField] private GameObject skipUI;

    private int phraseCount;

    [SerializeField] TypewriterByCharacter typeWriter;

    private bool inputActiveForIntro;
    private bool textShowed;

    private void Awake() {
        phraseText.gameObject.SetActive(false);
        anyKeyText.gameObject.SetActive(false);
        skipUI.SetActive(false);
    }

    private void Update() {

        if(!inputActiveForIntro) {
            anyKeyText.gameObject.SetActive(false);
            return;
        } 

        if(textShowed) {
            anyKeyText.gameObject.SetActive(true);
        } else {
            anyKeyText.gameObject.SetActive(false);
        }

        if (Input.anyKeyDown) {
            if(!textShowed) {
                typeWriter.SkipTypewriter();
            } else {
                NextPhrase();
            }
        }
    }

    public void StartTalking() {
        phraseText.gameObject.SetActive(true);
        anyKeyText.gameObject.SetActive(true);
        skipUI.SetActive(true);
        phraseCount = 0;
        typeWriter.ShowText(phrases[phraseCount]);

        inputActiveForIntro = true;
    }

    public void StopTalking() {
        phraseText.gameObject.SetActive(false);
        anyKeyText.gameObject.SetActive(false);
        skipUI.SetActive(false);

        LevelIntroCutScene.Instance.SetNPCIsTalking(false);


        inputActiveForIntro = false;
    }

    public void NextPhrase() {
        phraseCount++;
        if(phraseCount >= phrases.Count) {
            StopTalking();
            return;
        }
        if(phrases[phraseCount] == "camera") {
            LevelIntroCutScene.Instance.MoveCameraToNextPosition(.35f);
            return;
        }
        if (phrases[phraseCount] == "resetCamera") {
            LevelIntroCutScene.Instance.SetCameraToNPC();
            return;
        }
        typeWriter.ShowText(phrases[phraseCount]);
    }

    public void SetTextShowed(bool showed) {
        textShowed = showed;
    }

    public void SetInputActive(bool active) {
        inputActiveForIntro = active;
    }
 }
