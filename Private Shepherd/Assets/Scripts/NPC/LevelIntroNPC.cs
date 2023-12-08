using Febucci.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelIntroNPC : MonoBehaviour
{

    public static LevelIntroNPC Instance { get; private set; }

    public List<string> phrases = new List<string>();
    [SerializeField] private TextMeshProUGUI phraseText;

    private int phraseCount;
    private float defaultCameraSpeed = 8f;

    [SerializeField] TypewriterByCharacter typeWriter;
    [SerializeField] float[] cameraZooms;
    [SerializeField] float[] cameraSpeeds;
    int i;
    int cameraInt;

    private bool inputActiveForIntro;
    private bool textShowed;
    private bool introFinished;

    public event EventHandler OnTutorialPaused;

    private void Awake() {
        Instance = this;
        phraseText.gameObject.SetActive(false);
    }

    private void Update() {
        if (!inputActiveForIntro) {
            return;
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
        typeWriter.ShowText(phrases[phraseCount]);

        inputActiveForIntro = true;
    }

    public void StopTalking() {
        inputActiveForIntro = false;
        if(phraseText != null) {
            phraseText.gameObject.SetActive(false);
            LevelIntroCutScene.Instance.SetNPCIsTalking(false);
        }
        introFinished = true;
    }

    public void PauseTalking() {
        inputActiveForIntro = false;
        if (phraseText != null) {
            phraseText.gameObject.SetActive(false);
            LevelIntroCutScene.Instance.SetNPCIsTalking(false);
        }
    }

    public void NextPhrase() {
        phraseCount++;
        if(LevelIntroCutScene.Instance.GetCameraLocked()) {
            LevelIntroCutScene.Instance.SetCameraLocked(false);
        }
        if (phraseCount >= phrases.Count) {
            StopTalking();
            return;
        }
        if(phrases[phraseCount] == "camera") {
            float zoom = cameraZooms[cameraInt];
            float cameraSpeed = defaultCameraSpeed;
            if (cameraSpeeds[cameraInt] != 0) {
                cameraSpeed = cameraSpeeds[i];
            }
            i++;
            cameraInt++;
            LevelIntroCutScene.Instance.MoveCameraToNextPosition(cameraSpeed, zoom);
            return;
        }
        if (phrases[phraseCount] == "cameraLock") {
            float zoom = cameraZooms[i];
            float cameraSpeed = defaultCameraSpeed;
            if (cameraSpeeds[i] != 0) {
                cameraSpeed = cameraSpeeds[i];
            }
            i++;
            LevelIntroCutScene.Instance.LockCameraToNextPosition(cameraSpeed, zoom);
            return;
        }
        if (phrases[phraseCount] == "pauseTutorial") {
            i++;
            PauseTalking();
            OnTutorialPaused?.Invoke(this, EventArgs.Empty);
            TutorialManager.Instance.PlaySheep();
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

    public bool GetInputActiveForIntro() {
        return inputActiveForIntro;
    }

    public bool GetTextShowed() {
        return textShowed;
    }

    public bool GetIntroFinished() {
        return introFinished;
    }
 }
